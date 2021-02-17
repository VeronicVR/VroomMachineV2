using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;
using BlueRain;
using VroomMachineV2.Features;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Globalization;

namespace VroomMachineV2
{
    class Core
    {
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;
        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PAGE_READWRITE = 0x0004;

        public GodMode GodMode { get; private set; }
        public SpeedMultiplier SpeedMultiplier { get; private set; }
        public InfiniteAmmo InfiniteAmmo { get; private set; }
        public SpawnMoney MoneyHack { get; private set; }
        public ZombieHack ZombieHack { get; private set; }
        public XpMultiplier XpMultiplier { get; private set; }
        public MiscFeatures MiscFeatures { get; private set; }
        public CamoFeatures CamoFeatures { get; private set; }

        private const string GameTitle = "Call of Duty®: Black Ops Cold War";
        private const string ProcessName = "BlackOpsColdWar";

        private IntPtr _hWnd;
        private IntPtr _baseAddress;
        private NativeMemory _memory;
        private WpfConsole _console;

        private IntPtr _playerPtr;
        private IntPtr _playerPedPtr;
        private IntPtr _zmGlobalBase;
        private IntPtr _zmBotBase;
        private IntPtr _zmBotListBase;

        public Core(WpfConsole console)
        {
            _console = console;
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(UInt32 dwAccess, bool inherit, int pid);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, Int64 lpBaseAddress, [In, Out] byte[] lpBuffer, UInt64 dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, Int64 lpBaseAddress, [In, Out] byte[] lpBuffer, UInt64 dwSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, UInt32 dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, UInt32 dwSize, uint flNewProtect, out uint lpflOldProtect);

        private Process CurProcess;
        private IntPtr ProcessHandle;
        private int ProcessID;
        public IntPtr BaseModule;

        public bool AttackProcess(string _ProcessName)
        {
            Process[] Processes = Process.GetProcessesByName(_ProcessName);

            if (Processes.Length > 0)
            {
                BaseModule = Processes[0].MainModule.BaseAddress;
                CurProcess = Processes[0];
                ProcessID = Processes[0].Id;

                ProcessHandle = Core.OpenProcess(Core.PROCESS_VM_READ | Core.PROCESS_VM_WRITE | Core.PROCESS_VM_OPERATION, false, ProcessID);
                if (ProcessHandle != IntPtr.Zero)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsOpen()
        {
            if (ProcessName == string.Empty)
            {
                return false;
            }
            else
            {
                if (AttackProcess(ProcessName))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Start()
        {
            if ((_hWnd = WinAPI.FindWindowByCaption(_hWnd, GameTitle)) == IntPtr.Zero) 
                return false;

            Process[] processes = Process.GetProcessesByName(ProcessName);
            bool temp = Attach(processes[0]);

            if (temp)
            {

                GodMode = new GodMode(_baseAddress, _memory);
                SpeedMultiplier = new SpeedMultiplier(_baseAddress, _memory);
                InfiniteAmmo = new InfiniteAmmo(_baseAddress, _memory);
                MoneyHack = new SpawnMoney(_baseAddress, _memory);
                MiscFeatures = new MiscFeatures(_baseAddress, _memory);
                ZombieHack = new ZombieHack(_playerPedPtr, _zmBotListBase, _zmGlobalBase, _memory);
                XpMultiplier = new XpMultiplier(_baseAddress, _memory);
                //CamoFeatures = new CamoFeatures(_baseAddress, _zmBotListBase, _zmGlobalBase, _memory);

                return true;
            }

            return false;

        }

        private bool Attach(Process process)
        {
            _memory = new ExternalProcessMemory(process);
            _baseAddress = _memory.GetModule("BlackOpsColdWar.exe").BaseAddress;

            _playerPtr = _memory.Read<IntPtr>(_baseAddress + Offsets.PlayerBase);
            _playerPedPtr = _memory.Read<IntPtr>(_baseAddress + Offsets.PlayerBase + 0x8);
            _zmGlobalBase = _memory.Read<IntPtr>(_baseAddress + Offsets.PlayerBase + 0x60);
            _zmBotBase = _memory.Read<IntPtr>(_baseAddress + Offsets.PlayerBase + 0x68);
            _zmBotListBase = _memory.Read<IntPtr>(_zmBotBase + 0x8);

            if (_playerPedPtr == IntPtr.Zero || _zmGlobalBase == IntPtr.Zero || _zmBotBase == IntPtr.Zero || _zmBotListBase == IntPtr.Zero)
            {
                return false;
            }

            _console.WriteLine($"Attached! ", Brushes.Green);
            _console.WriteLine($"BaseAddress: 0x" + _baseAddress.ToString("X"), Brushes.Green); 
            _console.WriteLine($"PlayerCompPtr: 0x" + _playerPtr.ToString("X"), Brushes.Green);
            _console.WriteLine($"playerPedPtr: 0x" + _playerPedPtr.ToString("X"), Brushes.Green);
            _console.WriteLine($"zmGlobalBase: 0x" + _zmGlobalBase.ToString("X"), Brushes.Green);
            _console.WriteLine($"zmBotBase: 0x" + _zmBotBase.ToString("X"), Brushes.Green);
            _console.WriteLine($"zmBotListBase:0x" + _zmBotListBase.ToString("X"), Brushes.Green);
          
            return true;
        }
    }
}
