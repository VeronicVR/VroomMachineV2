using System;
using BlueRain;

namespace VroomMachineV2.Features
{
    class SpawnMoney
    {
        private IntPtr _baseAddress;
        private NativeMemory _memory;

        public SpawnMoney(IntPtr baseAddress, NativeMemory memory)
        {
            _baseAddress = baseAddress;
            _memory = memory;
        }

        public void InfiniteMoney()
        {
             _memory.Write<int>(true, 100000, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Points);
        }
    }
}
