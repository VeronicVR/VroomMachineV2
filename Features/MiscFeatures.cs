using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BlueRain;
using VroomMachineV2.Utils;

namespace VroomMachineV2.Features
{
    class MiscFeatures
    {
        private readonly IntPtr _baseAddress;
        private readonly NativeMemory _memory;

        private bool _infraredVision;
        private bool _allPerks;
        private bool _stupidFlagThatNeedsTooGetRefactored = true;
        private Queue _weaponQue;

        public Dictionary<int, string> weapons = new Dictionary<int, string>
        {
            {1, "Default Weapon"},
            {2, "Boobies"},
        };



        public MiscFeatures(IntPtr baseAddress, NativeMemory memory)
        {
            _baseAddress = baseAddress;
            _memory = memory;
        }

        public void ToggleInfraredVision()
        {
            if (!_infraredVision)
            {
                _infraredVision = !_infraredVision;
                _memory.Write<byte>(false, 0x10, _baseAddress + Offsets.PlayerBase,
                    (IntPtr) Offsets.PlayerCompPtr.InfraredVision); //0x10 = 16

            }
            else
            {
                _infraredVision = !_infraredVision;
                _memory.Write<byte>(false, 0x0, _baseAddress + Offsets.PlayerBase,
                    (IntPtr) Offsets.PlayerCompPtr.InfraredVision);
            }
        }

        public void DoRapidFire()
        {
            if (KeyUtils.GetKeyDown(0x1))
            {
                _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.RapidFire1);
                _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.RapidFire2);
            }
        }

        public void AllPerks()
        {
            if (!_allPerks)
            {
                _allPerks = !_allPerks;
                _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.Perk1);
                _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.Perk2);
                _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.Perk3);
                _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.Perk4);
                _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.Perk5);
                _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.Perk6);
                _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.Perk7);
                _memory.Write(false, -1, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.Perk8);
            }
            else
            {
                _allPerks = !_allPerks;
            }
        }

        public void SetWeapon(int id)
        {
            _memory.Write<int>(false, id, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.SetWeaponID /*+ 0x40*/);
        }

        public void AutomaticWeaponSwitch()
        {
            SetQueue();
            int killCount = _memory.Read<int>(false, _baseAddress + Offsets.PlayerBase, (IntPtr) Offsets.PlayerCompPtr.KillCount);

            if (killCount % 5 == 0)
            {
                SetWeapon((int)_weaponQue.Peek());
                _weaponQue.Dequeue();
            }
        }

        private void SetQueue()
        {
            if (_stupidFlagThatNeedsTooGetRefactored)
            {
                _weaponQue = new Queue();
                foreach (var weaponId in weapons.Keys)
                {
                    _weaponQue.Enqueue(weaponId);
                }

                _stupidFlagThatNeedsTooGetRefactored = false;
            }
        }
    }
}
