using System;

namespace VroomMachineV2
{
    class Offsets
    {
        public static int PlayerBase = 0x10A97348;
        public static int ZMXPScaleBase = 0x10AC7BC0;
        public static int XPScaleBase = 0x10AC8BC0; 
        public static int JumpHeightBase = 0x10B8E008;
        public static int CMDBufferBase = 0x12469150;
        public static int CMDBB_Exec = -0x1B;
        public static int TimeScaleBase = 0xFB557DC;                // Got XOR Obfusicated. Simple ^ or << >> parse and done :P they only waste Double Memory Usage with this...


        public static class PlayerCompPtr
        {
            public static int Name = 0x5BDA;                        //Player Name
            public static int ClanTags = 0x605C;                    // Player Clan/Crew-Tag

            public static int ArraySizeOffset = 0xB900;             // Size of Array between Players Data 
            public static int InfraredVision = 0xE66;               // (byte) On=0x10|Off=0x0
            public static int GodMode = 0xE67;                      // (byte) On=0xA0|Off=0x20
            public static int RunSpeed = 0x5C30;                    // (float)
            public static int Ammo = 0x13D4;                        // +(1-5 * 0x4 for WP1 to WP6) (WP0 Mostly used in MP, ZM first WP is WP1 | WP3-6 Mostly used for Granades and Special)
            public static int MaxAmmo = 0x1360;                     // +(1-5 * 0x8 for WP1 to WP6) (WP0 Mostly used in MP, ZM first WP is WP1 | WP3-6 Mostly used for Granades and Special)
            public static int Points = 0x5D04;
            public static int RapidFire1 = 0xE6C;
            public static int RapidFire2 = 0xE80;
            public static int CurrentUsedWeaponID = 0x28;           // Shows Current Used WeaponID (this is Read Only)
            public static int SetWeaponID = 0xB0;                   //The Game assign the next Free WP Slot so WP1 is MainWeapon, you get a granade, then WP2 is the Granade, you buy a Weapon from wall then this is WP3 and so on..

            public static int RapidFieldUpgrade_Offset = 0xF24;     // Weres the offset your reading this from?

            public static int Perk1 = 0x10CC;
            public static int Perk2 = 0x10D2;
            public static int Perk3 = 0x10E4;
            public static int Perk4 = 0x10E8;
            public static int Perk5 = 0x10C4;
            public static int Perk6 = 0x10C8;
            public static int Perk7 = 0x10D4;
            public static int Perk8 = 0x10D6;

            public static int KillCount = 0x5CE8;
        }


        public class PlayerPedPtr
        {
            public static int ArraySize_Offset = 0xB900;             // ArraySize to next Player.
            public static int Health = 0x398;
            public static int MaxHealth = 0x39C;                    // Max Health dont increase by using Perk Juggernog
            public static int Coords = 0x2D4;                       // Vector3 (X, Y, Z)
            public static int HeadingZ = 0x34;                      // float
            public static int HeadingXY = 0x38;                     // float | Can be used to TP Zombies in front of you by your Heading Position and Forward Distance.

        }

        public class ZombieBotListBase
        {
            public static int BotArraySizeOffset = 0x5F8;           // ArraySize to next Zombie.
            public static int BotHealth = 0x398;
            public static int BotMaxHealth = 0x39C;
            public static int Coords = 0x2D4;                       // Can be used to Teleport all Zombies in front of any Player with a Heading Variable from the Players.

        }

        public class ZombieGlobalClass
        {
            public static int ZM_Global_ZombiesIgnoreAll = 0x14;    // Zombies Ignore any Player in the Lobby.
            public static int ZombieLeftCount = 0x3C;
        }

        public class ZombieXpScaleBase
        {
            public static int XPGun = 0x30;                         //XPGun_Offset
            public static int XPUserReal = 0x28;                    //Real XPEP_RealAdd_Offset
        }
    }
}
