using System;
using BlueRain;

namespace VroomMachineV2.Features
{
    class InfiniteAmmo
    {
        private IntPtr _baseAddress;
        private NativeMemory _memory;
        public InfiniteAmmo(IntPtr baseAddress, NativeMemory memory)
        {
            _baseAddress = baseAddress;
            _memory = memory;
        }

        public void DoInfiniteAmmo()
        {
            for (int i = 1; i < 6; i++)
            {
                _memory.Write(false, 30, _baseAddress + Offsets.PlayerBase, (IntPtr)Offsets.PlayerCompPtr.Ammo + (i * 0x4));
            }
        }
    }
}
