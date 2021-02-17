using System;
using BlueRain;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Threading;
using System.Windows;
using MahApps.Metro.Controls;
using System.Windows.Media;
using VroomMachineV2.TODO;
using System.Windows.Threading;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace VroomMachineV2
{
    public partial class MainWindow : MetroWindow
    {
        private readonly DispatcherTimer _TitleRenameTimer;
        private readonly DispatcherTimer _TPP_TPToTimer;

        private static IntPtr _baseAddress;
        private NativeMemory _memory;

        private WpfConsole _console;
        private Core _core;

        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();

        #region |- Main Window -|
        public MainWindow()
        {
            _TitleRenameTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) }; _TitleRenameTimer.Tick += TitleRename_Tick;
            _TPP_TPToTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(.1) }; _TPP_TPToTimer.Tick += TPP_TPTo_Tick;

            InitializeComponent();

            TeleLocations.Items.Clear();
            string[] lines = null;
            try
            {
                string srcFilePath = "Utils/tp.txt";
                string rootPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                string fullPath = System.IO.Path.Combine(rootPath, srcFilePath);
                string filePath = new Uri(fullPath).LocalPath;

                lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (i % 5 == 0)
                        TeleLocations.Items.Add(lines[i]);
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File 'tp.txt' not found");
            }
            _TitleRenameTimer.Start();
            _TPP_TPToTimer.Start();
        }
        #endregion

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            return new string((from s in Enumerable.Repeat("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz#@$^*()", length)
                               select s[random.Next(s.Length)]).ToArray());
        }

        #region |- Timers -|

        //Make Label Update Timer Here!

        private void TitleRename_Tick(object source, EventArgs e)
        { Vroom.Title = RandomString(25); }

        private void TPP_TPTo_Tick(object source, EventArgs e)
        {
            /*if (metroRadioButton1.Checked)
            {
                Xvaldisplay.Text = "X: " + X;
                Yvaldisplay.Text = "Y: " + Y;
                Zvaldisplay.Text = "Z: " + Z;
            }
            else if (metroRadioButton2.Checked)
            {
                Xvaldisplay.Text = "X: " + p2X;
                Yvaldisplay.Text = "Y: " + p2Y;
                Zvaldisplay.Text = "Z: " + p2Z;
            }
            else if (metroRadioButton3.Checked)
            {
                Xvaldisplay.Text = "X: " + p3X;
                Yvaldisplay.Text = "Y: " + p3Y;
                Zvaldisplay.Text = "Z: " + p3Z;
            }
            else if (metroRadioButton4.Checked)
            {
                Xvaldisplay.Text = "X: " + p4X;
                Yvaldisplay.Text = "Y: " + p4Y;
                Zvaldisplay.Text = "Z: " + p4Z;
            }*/

            if (TPP1.IsChecked == true)
            {
                ToP1.IsEnabled = false;
                ToP1.IsChecked = false;
                ToP2.IsEnabled = true;
                ToP3.IsEnabled = true;
                ToP4.IsEnabled = true;
            }
            else if (TPP2.IsChecked == true)
            {
                ToP1.IsEnabled = true;
                ToP2.IsEnabled = false;
                ToP2.IsChecked = false;
                ToP3.IsEnabled = true;
                ToP4.IsEnabled = true;
            }
            else if (TPP3.IsChecked == true)
            {
                ToP1.IsEnabled = true;
                ToP2.IsEnabled = true;
                ToP3.IsEnabled = false;
                ToP3.IsChecked = false;
                ToP4.IsEnabled = true;
            }
            else if (TPP4.IsChecked == true)
            {
                ToP1.IsEnabled = true;
                ToP2.IsEnabled = true;
                ToP3.IsEnabled = true;
                ToP4.IsEnabled = false;
                ToP4.IsChecked = false;
            }
        }

        #endregion

        private void ConClear(object sender, RoutedEventArgs e)
        { _console.Clear(); ConsoleBox.Text = "Call of Duty: Black Ops Cold War Trainer | Version 1.0.0 | By Riko | LOADED!"; }

        class address
        {
            public static void setadd()
            {
                _baseAddress = (IntPtr)WinAPI.GetBaseAddress("BlackOpsColdWar").ToInt64();
            }
            WinAPI m = new WinAPI();
        }

        private void Launch(object sender, RoutedEventArgs e)
        {
            _core = new Core(_console);
            if (_core.Start())
            {
                Connection.Content = "Connected";
                Connection.Foreground = Brushes.Lime;
                EnableContentOnWindow();
                _backgroundWorker.DoWork += BackgroundWorkerDoWork;
                _backgroundWorker.RunWorkerAsync();
            }
            else
            {
                try
                {
                    Thread.Sleep(100);
                    if (_core.IsOpen())
                    {
                        Connection.Content = "Game Found | Press again while in match";
                        Connection.Foreground = Brushes.Blue;
                        address.setadd();
                    }
                    else
                    {
                        Connection.Content = "Game Not Found";
                        Connection.Foreground = Brushes.Red;
                        _console.WriteLine("Game is not open!", Brushes.Red);
                    }
                }
                catch
                {
                    _console.WriteLine("Failed! How did you even make this show up?", Brushes.Red);
                }
            }
        }

        private void EnableContentOnWindow() //Once you press connect, enables all object to be interactable | Line 43
        {

            #region |- HOST Player 1 -|
            //Player 1/Host
            P1GodMode.IsEnabled = true;
            P1Ammo.IsEnabled = true;
            P1NoTarget.IsEnabled = true;
            P1RapidFire.IsEnabled = true;
            P1RapidSkill.IsEnabled = true;
            P1Perks.IsEnabled = true;
            P1Cash.IsEnabled = true;
            CashUpDown.IsEnabled = true;
            P1CashLock.IsEnabled = true;

            GiveWeapon.IsEnabled = true;
            Slot1.IsEnabled = true;
            Slot2.IsEnabled = true;
            Slot3.IsEnabled = true;
            Slot4.IsEnabled = true;
            Slot5.IsEnabled = true;
            Slot6.IsEnabled = true;
            #endregion


            #region |- Player 2 -|
            //Player 2
            P2GodMode.IsEnabled = true;
            P2Ammo.IsEnabled = true;
            P2NoTarget.IsEnabled = true;
            P2RapidFire.IsEnabled = true;
            P2RapidSkill.IsEnabled = true;
            P2Perks.IsEnabled = true;
            P2Cash.IsEnabled = true;
            p2CashUpDown.IsEnabled = true;
            P2CashLock.IsEnabled = true;

            p2GiveWeapon.IsEnabled = true;
            p2Slot1.IsEnabled = true;
            p2Slot2.IsEnabled = true;
            p2Slot3.IsEnabled = true;
            p2Slot4.IsEnabled = true;
            p2Slot5.IsEnabled = true;
            p2Slot6.IsEnabled = true;
            #endregion


            #region |- Player 3 -|
            //Player 3
            P3GodMode.IsEnabled = true;
            P3Ammo.IsEnabled = true;
            P3NoTarget.IsEnabled = true;
            P3RapidFire.IsEnabled = true;
            P3RapidSkill.IsEnabled = true;
            P3Perks.IsEnabled = true;
            P3Cash.IsEnabled = true;
            p3CashUpDown.IsEnabled = true;
            P3CashLock.IsEnabled = true;

            p3GiveWeapon.IsEnabled = true;
            p3Slot1.IsEnabled = true;
            p3Slot2.IsEnabled = true;
            p3Slot3.IsEnabled = true;
            p3Slot4.IsEnabled = true;
            p3Slot5.IsEnabled = true;
            p3Slot6.IsEnabled = true;
            #endregion


            #region |- Player 4 -|
            //Player 4
            P4GodMode.IsEnabled = true;
            P4Ammo.IsEnabled = true;
            P4NoTarget.IsEnabled = true;
            P4RapidFire.IsEnabled = true;
            P4RapidSkill.IsEnabled = true;
            P4Perks.IsEnabled = true;
            P4Cash.IsEnabled = true;
            p4CashUpDown.IsEnabled = true;
            P4CashLock.IsEnabled = true;

            p4GiveWeapon.IsEnabled = true;
            p4Slot1.IsEnabled = true;
            p4Slot2.IsEnabled = true;
            p4Slot3.IsEnabled = true;
            p4Slot4.IsEnabled = true;
            p4Slot5.IsEnabled = true;
            p4Slot6.IsEnabled = true;
            #endregion


            #region |- Teleport -|
            //Teleport
            //TeleLocations.IsEnabled = true;
            NoclipToggle.IsEnabled = true;
            TeleportToXYZ.IsEnabled = true;
            Player1Radio.IsEnabled = true;
            Player2Radio.IsEnabled = true;
            Player3Radio.IsEnabled = true;
            Player4Radio.IsEnabled = true;
            XUpDown.IsEnabled = true;
            YUpDown.IsEnabled = true;
            ZUpDown.IsEnabled = true;
            TPP1.IsEnabled = true;
            TPP2.IsEnabled = true;
            TPP3.IsEnabled = true;
            TPP4.IsEnabled = true;
            ToP1.IsEnabled = true;
            ToP2.IsEnabled = true;
            ToP3.IsEnabled = true;
            ToP4.IsEnabled = true;
            TeleToPlr.IsEnabled = true;
            #endregion


            #region |- Extras -|
            //Extras
            SetZMHP.IsEnabled = true;
            ZMHPUpDown.IsEnabled = true;
            LockZMHP.IsEnabled = true;
            ZMTP.IsEnabled = true;
            P1ZM.IsEnabled = true;
            P2ZM.IsEnabled = true;
            P3ZM.IsEnabled = true;
            P4ZM.IsEnabled = true;
            FreezeZombies.IsEnabled = true;
            #endregion

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        { _console = new WpfConsole(ConsoleBox); }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    #region |- Player 1 Loops -|
                    if (_P1FreezeMoney) _memory.Write<int>(true, (int)CashUpDown.Value, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Points);
                    if (_P1SetAmmo) for (int i = 1; i < 6; i++) { _memory.Write(false, 30, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Ammo + (i * 0x4)); }
                    if (_P1SetNoTarget) _memory.Write<int>(true, 0, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerPedPtr.Health);
                    if (_P1SetRapidFire)
                        if (Utils.KeyUtils.GetKeyDown(0x1))
                        {
                            _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.RapidFire1);
                            _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.RapidFire2);
                        };
                    if (_P1SetRapidSkill) 
                       _memory.Write(false, 1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.RapidFieldUpgrade_Offset);
                    if (_P1SetPerks)

                    #region |- Set Perk List -|
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Perk1);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Perk2);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Perk3);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Perk4);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Perk5);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Perk6);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Perk7);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Perk8);
                    #endregion
                    #endregion

                    #region |- Player 2 Loops -|
                    if (_P2FreezeMoney) _memory.Write<int>(true, (int)CashUpDown.Value, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.Points);
                    if (_P2SetAmmo) for (int i = 1; i < 6; i++) { _memory.Write(false, 30, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.Ammo + (i * 0x4)); }
                    if (_P2SetNoTarget) _memory.Write<int>(true, 0, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerPedPtr.Health);
                    if (_P2SetRapidFire)
                        if (Utils.KeyUtils.GetKeyDown(0x1))
                        {
                            _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.RapidFire1);
                            _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.RapidFire2);
                        };
                    if (_P2SetRapidSkill)
                        _memory.Write(false, 1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.RapidFieldUpgrade_Offset);
                    if (_P2SetPerks)
                        #region |- Set Perk List -|
                        _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.Perk1);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.Perk2);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.Perk3);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.Perk4);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.Perk5);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.Perk6);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.Perk7);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.Perk8);
                    #endregion
                    #endregion

                    #region |- Player 3 Loops -|
                    if (_P3FreezeMoney) _memory.Write<int>(true, (int)CashUpDown.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.Points);
                    if (_P3SetAmmo) for (int i = 1; i < 6; i++) { _memory.Write(false, 30, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.Ammo + (i * 0x4)); }
                    if (_P3SetNoTarget) _memory.Write<int>(true, 0, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerPedPtr.Health);
                    if (_P3SetRapidFire)
                        if (Utils.KeyUtils.GetKeyDown(0x1))
                        {
                            _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.RapidFire1);
                            _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.RapidFire2);
                        };
                    if (_P3SetRapidSkill)
                        _memory.Write(false, 1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.RapidFieldUpgrade_Offset);
                    if (_P3SetPerks)
                        #region |- Set Perk List -|
                        _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.Perk1);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.Perk2);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.Perk3);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.Perk4);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.Perk5);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.Perk6);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.Perk7);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.Perk8);
                    #endregion
                    #endregion

                    #region |- Player 4 Loops -|
                    if (_P4FreezeMoney) _memory.Write<int>(true, (int)CashUpDown.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.Points);
                    if (_P4SetAmmo) for (int i = 1; i < 6; i++) { _memory.Write(false, 30, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.Ammo + (i * 0x4)); }
                    if (_P4SetNoTarget) _memory.Write<int>(true, 0, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerPedPtr.Health);
                    if (_P4SetRapidFire)
                        if (Utils.KeyUtils.GetKeyDown(0x1))
                        {
                            _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.RapidFire1);
                            _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.RapidFire2);
                        };
                    if (_P4SetRapidSkill)
                        _memory.Write(false, 1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.RapidFieldUpgrade_Offset);
                    if (_P4SetPerks)
                        #region |- Set Perk List -|
                        _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.Perk1);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.Perk2);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.Perk3);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.Perk4);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.Perk5);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.Perk6);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.Perk7);
                    _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.Perk8);
                    #endregion
                    #endregion

                    if (_teleportZombies) _core.ZombieHack.TeleportZombies(true, 150);
                    if (_teleportZombiesLocation) _core.ZombieHack.TeleportZombies(false);


                    //Set labels and enables here
                    //Player 1


                }
                catch (Exception exception)
                {
                    _console.WriteLine(exception.Message, Brushes.Red);
                }

            }
        }
        private void TimerEnable(object sender, RoutedEventArgs e)
        {
            _console.WriteLine("Timer Enabled", Brushes.Green);

        }

        #region |- Private Bool List -|

        #region |- Host Bool List -|

        private bool _P1FreezeMoney;
        private bool _P1SetAmmo;
        private bool _P1SetNoTarget;
        private bool _P1SetRapidFire;
        private bool _P1SetRapidSkill;
        private bool _P1SetPerks;

        #endregion

        #region |- Player 2 Bool List -|

        private bool _P2FreezeMoney;
        private bool _P2SetAmmo;
        private bool _P2SetNoTarget;
        private bool _P2SetRapidFire;
        private bool _P2SetRapidSkill;
        private bool _P2SetPerks;

        #endregion

        #region |- Player 3 Bool List -|

        private bool _P3FreezeMoney;
        private bool _P3SetAmmo;
        private bool _P3SetNoTarget;
        private bool _P3SetRapidFire;
        private bool _P3SetRapidSkill;
        private bool _P3SetPerks;

        #endregion

        #region |- Player 4 Bool List -|

        private bool _P4FreezeMoney;
        private bool _P4SetAmmo;
        private bool _P4SetNoTarget;
        private bool _P4SetRapidFire;
        private bool _P4SetRapidSkill;
        private bool _P4SetPerks;

        #endregion


        private bool _instantKill;
        private bool _teleportZombies;
        private bool _rapidFire;
        private bool _teleportZombiesLocation;
        private bool _allPerks;
        private bool _oneShotKill;

        #endregion

        #region |- Set HOST Player 1 -|
        //Player 1 HOST
        private void SetP1GodMode(object sender, RoutedEventArgs e)
        {
            if (P1GodMode.IsChecked == true)
            { _memory.Write<byte>(false, 0xA0, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.GodMode); }
            else
            { _memory.Write<byte>(false, 0x20, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.GodMode); }
        }
        private void SetP1Money(object sender, RoutedEventArgs e)
        { _memory.Write<Int64>(false, (int)CashUpDown.Value, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.GodMode); }
        private void FreezeP1Money(object sender, RoutedEventArgs e)
        {
            if (P1CashLock.IsChecked == true)
            { _P1FreezeMoney = true; }
            else
            { _P1FreezeMoney = false; }
        }
        private void SetP1Ammo(object sender, RoutedEventArgs e)
        {
            if (P1Ammo.IsChecked == true)
            { _P1SetAmmo = true; }
            else
            { _P1SetAmmo = false; }
        }
        private void SetP1NoTarget(object sender, RoutedEventArgs e)
        {
            if (P1NoTarget.IsChecked == true)
            { _P1SetNoTarget = true; }
            else
            { _P1SetNoTarget = false; }
        }
        private void SetP1RapidFire(object sender, RoutedEventArgs e)
        {
            if (P1RapidFire.IsChecked == true)
            { _P1SetRapidFire = true; }
            else
            { _P1SetRapidFire = false; }
        }
        private void SetP1RapidSkill(object sender, RoutedEventArgs e)
        {
            if (P1RapidFire.IsChecked == true)
            { _P1SetRapidSkill = true; }
            else
            { _P1SetRapidSkill = false; _memory.Write(false, 0, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.RapidFieldUpgrade_Offset); }
        }
        private void SetP1Perks(object sender, RoutedEventArgs e)
        {
            if (P1RapidFire.IsChecked == true)
            { _P1SetPerks = true; }
            else
            { _P1SetPerks = false; }
        }
        private void SetP1WeaponSlot(object sender, RoutedEventArgs e)
        {
            #region |- Set Weapon Slots -|
            if (Slot1.IsChecked == true)
            { _memory.Write<int>(false, (int)p1id.Value, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID); } // + 0x40 for each weapon
            else if (Slot2.IsChecked == true)
            { _memory.Write<int>(false, (int)p1id.Value, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x40); }
            else if (Slot3.IsChecked == true)
            { _memory.Write<int>(false, (int)p1id.Value, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x80); }
            else if (Slot4.IsChecked == true)
            { _memory.Write<int>(false, (int)p1id.Value, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0xC0); }
            else if (Slot5.IsChecked == true)
            { _memory.Write<int>(false, (int)p1id.Value, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x100); }
            else if (Slot6.IsChecked == true)
            { _memory.Write<int>(false, (int)p1id.Value, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x140); }
            #endregion
        }
        #endregion

        #region |- Set Player 2 -|
        //Player 2
        private void SetP2GodMode(object sender, RoutedEventArgs e)
        {
            if (P2GodMode.IsChecked == true)
            { _memory.Write<byte>(false, 0xA0, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.GodMode); }
            else
            { _memory.Write<byte>(false, 0x20, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.GodMode); }
        }
        private void SetP2Money(object sender, RoutedEventArgs e)
        { _memory.Write<Int64>(false, (int)CashUpDown.Value, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.GodMode); }
        private void FreezeP2Money(object sender, RoutedEventArgs e)
        {
            if (P2CashLock.IsChecked == true)
            { _P2FreezeMoney = true; }
            else
            { _P2FreezeMoney = false; }
        }
        private void SetP2Ammo(object sender, RoutedEventArgs e)
        {
            if (P2Ammo.IsChecked == true)
            { _P2SetAmmo = true; }
            else
            { _P2SetAmmo = false; }
        }
        private void SetP2NoTarget(object sender, RoutedEventArgs e)
        {
            if (P2NoTarget.IsChecked == true)
            { _P2SetNoTarget = true; }
            else
            { _P2SetNoTarget = false; }
        }
        private void SetP2RapidFire(object sender, RoutedEventArgs e)
        {
            if (P2RapidFire.IsChecked == true)
            { _P2SetRapidFire = true; }
            else
            { _P2SetRapidFire = false; }
        }
        private void SetP2RapidSkill(object sender, RoutedEventArgs e)
        {
            if (P2RapidFire.IsChecked == true)
            { _P2SetRapidSkill = true; }
            else
            { _P2SetRapidSkill = false; _memory.Write(false, 0, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.RapidFieldUpgrade_Offset); }
        }
        private void SetP2Perks(object sender, RoutedEventArgs e)
        {
            if (P2RapidFire.IsChecked == true)
            { _P2SetPerks = true; }
            else
            { _P2SetPerks = false; }
        }
        private void SetP2WeaponSlot(object sender, RoutedEventArgs e)
        {
            #region |- Set Weapon Slots -|
            if (p2Slot1.IsChecked == true)
            { _memory.Write<int>(false, (int)p2id.Value, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID); } // + 0x40 for each weapon
            else if (p2Slot2.IsChecked == true)
            { _memory.Write<int>(false, (int)p2id.Value, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x40); }
            else if (p2Slot3.IsChecked == true)
            { _memory.Write<int>(false, (int)p2id.Value, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x80); }
            else if (p2Slot4.IsChecked == true)
            { _memory.Write<int>(false, (int)p2id.Value, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0xC0); }
            else if (p2Slot5.IsChecked == true)
            { _memory.Write<int>(false, (int)p2id.Value, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x100); }
            else if (p2Slot6.IsChecked == true)
            { _memory.Write<int>(false, (int)p2id.Value, _baseAddress + Offsets.PlayerBase + Offsets.PlayerCompPtr.ArraySizeOffset, (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x140); }
            #endregion
        }
        #endregion

        #region |- Set Player 3 -|
        //Player 3
        private void SetP3GodMode(object sender, RoutedEventArgs e)
        {
            if (P3GodMode.IsChecked == true)
            { _memory.Write<byte>(false, 0xA0, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.GodMode); }
            else
            { _memory.Write<byte>(false, 0x20, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.GodMode); }
        }
        private void SetP3Money(object sender, RoutedEventArgs e)
        { _memory.Write<Int64>(false, (int)CashUpDown.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.GodMode); }
        private void FreezeP3Money(object sender, RoutedEventArgs e)
        {
            if (P3CashLock.IsChecked == true)
            { _P3FreezeMoney = true; }
            else
            { _P3FreezeMoney = false; }
        }
        private void SetP3Ammo(object sender, RoutedEventArgs e)
        {
            if (P3Ammo.IsChecked == true)
            { _P3SetAmmo = true; }
            else
            { _P3SetAmmo = false; }
        }
        private void SetP3NoTarget(object sender, RoutedEventArgs e)
        {
            if (P3NoTarget.IsChecked == true)
            { _P3SetNoTarget = true; }
            else
            { _P3SetNoTarget = false; }
        }
        private void SetP3RapidFire(object sender, RoutedEventArgs e)
        {
            if (P3RapidFire.IsChecked == true)
            { _P3SetRapidFire = true; }
            else
            { _P3SetRapidFire = false; }
        }
        private void SetP3RapidSkill(object sender, RoutedEventArgs e)
        {
            if (P3RapidFire.IsChecked == true)
            { _P3SetRapidSkill = true; }
            else
            { _P3SetRapidSkill = false; _memory.Write(false, 0, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.RapidFieldUpgrade_Offset); }
        }
        private void SetP3Perks(object sender, RoutedEventArgs e)
        {
            if (P3RapidFire.IsChecked == true)
            { _P3SetPerks = true; }
            else
            { _P3SetPerks = false; }
        }
        private void SetP3WeaponSlot(object sender, RoutedEventArgs e)
        {
            #region |- Set Weapon Slots -|
            if (p3Slot1.IsChecked == true)
            { _memory.Write<int>(false, (int)p3id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID); } // + 0x40 for each weapon
            else if (p3Slot2.IsChecked == true)
            { _memory.Write<int>(false, (int)p3id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x40); }
            else if (p3Slot3.IsChecked == true)
            { _memory.Write<int>(false, (int)p3id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x80); }
            else if (p3Slot4.IsChecked == true)
            { _memory.Write<int>(false, (int)p3id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0xC0); }
            else if (p3Slot5.IsChecked == true)
            { _memory.Write<int>(false, (int)p3id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x100); }
            else if (p3Slot6.IsChecked == true)
            { _memory.Write<int>(false, (int)p3id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 2), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x140); }
            #endregion
        }
        #endregion

        #region |- Set Player 4 -|
        //Player 4
        private void SetP4GodMode(object sender, RoutedEventArgs e)
        {
            if (P4GodMode.IsChecked == true)
            { _memory.Write<byte>(false, 0xA0, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.GodMode); }
            else
            { _memory.Write<byte>(false, 0x20, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.GodMode); }
        }
        private void SetP4Money(object sender, RoutedEventArgs e)
        { _memory.Write<Int64>(false, (int)CashUpDown.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.GodMode); }
        private void FreezeP4Money(object sender, RoutedEventArgs e)
        {
            if (P4CashLock.IsChecked == true)
            { _P4FreezeMoney = true; }
            else
            { _P4FreezeMoney = false; }
        }
        private void SetP4Ammo(object sender, RoutedEventArgs e)
        {
            if (P4Ammo.IsChecked == true)
            { _P4SetAmmo = true; }
            else
            { _P4SetAmmo = false; }
        }
        private void SetP4NoTarget(object sender, RoutedEventArgs e)
        {
            if (P4NoTarget.IsChecked == true)
            { _P4SetNoTarget = true; }
            else
            { _P4SetNoTarget = false; }
        }
        private void SetP4RapidFire(object sender, RoutedEventArgs e)
        {
            if (P4RapidFire.IsChecked == true)
            { _P4SetRapidFire = true; }
            else
            { _P4SetRapidFire = false; }
        }
        private void SetP4RapidSkill(object sender, RoutedEventArgs e)
        {
            if (P4RapidFire.IsChecked == true)
            { _P4SetRapidSkill = true; }
            else
            { _P4SetRapidSkill = false; _memory.Write(false, 0, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.RapidFieldUpgrade_Offset); }
        }
        private void SetP4Perks(object sender, RoutedEventArgs e)
        {
            if (P4RapidFire.IsChecked == true)
            { _P4SetPerks = true; }
            else
            { _P4SetPerks = false; }
        }
        private void SetP4WeaponSlot(object sender, RoutedEventArgs e)
        {
            #region |- Set Weapon Slots -|
            if (p4Slot1.IsChecked == true)
            { _memory.Write<int>(false, (int)p4id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID); } // + 0x40 for each weapon
            else if (p4Slot2.IsChecked == true)
            { _memory.Write<int>(false, (int)p4id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x40); }
            else if (p4Slot3.IsChecked == true)
            { _memory.Write<int>(false, (int)p4id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x80); }
            else if (p4Slot4.IsChecked == true)
            { _memory.Write<int>(false, (int)p4id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0xC0); }
            else if (p4Slot5.IsChecked == true)
            { _memory.Write<int>(false, (int)p4id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x100); }
            else if (p4Slot6.IsChecked == true)
            { _memory.Write<int>(false, (int)p4id.Value, _baseAddress + Offsets.PlayerBase + (Offsets.PlayerCompPtr.ArraySizeOffset * 3), (IntPtr)Offsets.PlayerCompPtr.SetWeaponID + 0x140); }
            #endregion
        }
        #endregion

        private void TeleportZombiesEnabled(object sender, RoutedEventArgs e)
        {
            //if (TeleportZombiePositionCheckBox.IsChecked.GetValueOrDefault())
            //    TeleportZombiePositionCheckBox.IsChecked = false;

            _console.WriteLine("Teleport Zombies Too Crosshair Enabled", Brushes.Green);
            _teleportZombies = true;
        }

        private void TeleportZombiesDisable(object sender, RoutedEventArgs e)
        {
            _console.WriteLine("Teleport Zombies Disabled", Brushes.Green);
            _teleportZombies = false;
        }




        private void SetPosition(object sender, RoutedEventArgs e)
        {
            Vector3 position = _core.ZombieHack.SetPosition();
            //PositionLabel.Content = $"Set Position: [{position.X},{position.Y},{position.Z}]";
        }

        private void TeleportZombiesPosEnabled(object sender, RoutedEventArgs e)
        {
            _console.WriteLine("Teleporting Zombies To Location Enabled", Brushes.Green);

            //if (TeleportZombieCheckBox.IsChecked.GetValueOrDefault())
            //    TeleportZombieCheckBox.IsChecked = false;

            _teleportZombiesLocation = true;
        }

        private void TeleportZombiesPosDisabled(object sender, RoutedEventArgs e)
        {
            _console.WriteLine("Teleporting Zombies To Location Disable", Brushes.Green);
            _teleportZombiesLocation = false;
        }

        private void ChangeWeaponButton_Click(object sender, RoutedEventArgs e)
        {
            _console.WriteLine("Weapon Changed", Brushes.Green);
            //KeyValuePair<int, string> weapon = (KeyValuePair<int, string>)WeaponIdComboBox.SelectedItem;
            //_core.MiscFeatures.SetWeapon(weapon.Key);
            //MyWeaponLabel.Content = $"Weapon: {WeaponIdComboBox.Text}";
        }

        private void OneShotGoldEnabled(object sender, RoutedEventArgs e)
        {
            _console.WriteLine("One Shot Gold Enabled", Brushes.Green);
            _oneShotKill = true;
        }

        private void OneShotGoldDisabled(object sender, RoutedEventArgs e)
        {
            _console.WriteLine("One Shot Gold Disabled", Brushes.Red);
            _oneShotKill = false;
        }

        private void XValCopy(object sender, RoutedEventArgs e)
        { Clipboard.SetText((string)Xvaldisplay.Content); }
        private void YValCopy(object sender, RoutedEventArgs e)
        { Clipboard.SetText((string)Yvaldisplay.Content); }
        private void ZValCopy(object sender, RoutedEventArgs e)
        { Clipboard.SetText((string)Zvaldisplay.Content); }
        private void WeapIDCopy(object sender, RoutedEventArgs e)
        { Clipboard.SetText((string)WeapID.Content); }

        private void TeleLocations_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string dir = String.Empty;
            string[] lines = null;
            try
            {
                string srcFilePath = "Utils/tp.txt";
                string rootPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                string fullPath = System.IO.Path.Combine(rootPath, srcFilePath);
                string filePath = new Uri(fullPath).LocalPath;

                lines = File.ReadAllLines(filePath);
                int index = Array.IndexOf(lines, TeleLocations.SelectedItem.ToString());

                Double.TryParse(lines[index + 1], out double X); Double.TryParse(lines[index + 2], out double Y); Double.TryParse(lines[index + 3], out double Z);
                XUpDown.Value = X;
                YUpDown.Value = Y;
                ZUpDown.Value = Z;
            }
            catch (FileNotFoundException)
            {
                _console.WriteLine("File 'tp.txt' not found", Brushes.Green);
            }
        }
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void LaunchReleaseSite(object sender, RoutedEventArgs e)
        { var psi = new ProcessStartInfo { FileName = "https://www.unknowncheats.me/forum/members/2493890.html", UseShellExecute = true }; Process.Start(psi); }
    }
}
