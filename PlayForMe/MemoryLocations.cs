using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryAddresses
{
    public class MemoryLocation
    {
        // LocalPlayer
        public int _Address_LocalPlayer_Health = 0xFA4980;             // Current Health
        public int _Address_LocalPlayer_HealthMax = 0xFA497c;          // Maximum Health
        public int _Address_LocalPlayer_Mana = 0xFA4988;               // Current Mana
        public int _Address_LocalPlayer_ManaMax = 0xF9BCB4;            // Maximum Mana
        public int _Address_LocalPlayer_Exp = 0xF9BCA0;                // Current Experience
        public int _Address_LocalPlayer_ExpMax = 0xF9BC90;             // Maximum Experience
        public int _Address_LocalPlayer_DP = 0xFA498E;                 // Current DP value

        public int _Address_LocalPlayer_Level = 0xF9BC88;              // Current Level
        public int _Address_LocalPlayer_HasTarget = 0xB303A4;          // Returns 1 if we have a target  
        public int _Address_LocalPlayer_Name = 0xF9BA74;               // Our Name
        public int _Address_LocalPlayer_X = 0xF9B654;                  // Player X Position
        public int _Address_LocalPlayer_Y = 0xF9B658;                  // Player Y Position

        public int _Address_LocalPlayer_ClassID = 0xFA49F4;

        public int _Address_LocalPlayer_CastingTime = 0xF9BF10;
        public int _Address_LocalPlayer_ID = 0xF9BA70;
        public int _Address_LocalPlayer_Stance = 0x4A18;        // Needs correcting
            public int[] offsets_pStance = { 0x220, 0x2a0 };

            public int LootConfirmation = 0xD2AAFC;             // Needs correcting
            public int[] offsets_LootConfirm = { 0x364 };

        public int _Address_LocalPlayer_DebffCount = 0x1018138;
            public int[] offsets_Debuffcount = { 0x5b4 };

        public int _Address_LocalPlayer_Following = 0xF9A840;

        // Chat
        public int _Chat_Whispers = 0x186890;
        public int _Chat_ObjectBlocking = 0x1834D8;

        // Other
        public int DialogueBox_SoulHeal = 0xCC20B0;

        // Skillbar
        public int ChanSkill_Indicator_Time = 0xD21564;         // Needs correcting
            public int[] offsets_ChainSkill_Time = { 0xb8 };

        public int ChainSkill_ID = 0xF9A834;
            public int[] offsets_ChainSkill_ID = { 0x8b0 };

        // Target
        public int _Address_Target_Ptr = 0xB3039C;
        public int _Address_TargetInCombat = 0xB3039C;
            public int[] offsets_TargetInCombat = { 0x370 };

        public int[] offset_TargetofTarget_ID = { 0x254, 0x330 };
        public int[] offset_Target_Health = { 0x254, 0x11d4 };
        public int[] offset_Target_HealthMax = { 0x254, 0x11d0 };
        public int[] offset_Target_Name = { 0x254, 0x3a };
        public int[] offset_Target_Level = { 0x254, 0x36 };
        public int[] offset_Target_Type = { 0x160 };
        public int[] offsets_Target_ID = { 0x254, 0x24 };
        public int[] offsets_Target_X = { 0x34 };
        public int[] offsets_Target_Y = { 0x38 };
        //public int[] offsets_TargetHasTarget = { 0x220, 0x2a0 };
        public int[] offsets_TargetIsMoving = { 0x124 };

        public int[] offsets_TargetIsCasting = { 0x254, 0xab4 };

        // Camera
        public int _Address_CameraX = 0xF9B22c;
        public int _Address_CameraY = 0xF9B234;
        public int _Address_AutoRunFlag = 0xF9B218;

        // Needs fixing
        public int _Address_Cube = 0xCBCF94;
            public int[] offsets_UsedCubeSlots = { 0x770 };
            public int[] offsets_MaxCubeSlots = { 0x764 };
    };
}
