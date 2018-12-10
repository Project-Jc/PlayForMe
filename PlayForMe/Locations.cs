using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryAddresses
{
    public class MemoryLocation
    {
        // LocalPlayer
        //
        public int _Address_LocalPlayer_Health = 0xCC78A8;             // Current Health
        public int _Address_LocalPlayer_HealthMax = 0xCC78A4;          // Maximum Health

        public int _Address_LocalPlayer_Mana = 0xCC78B0;               // Current Mana
        public int _Address_LocalPlayer_ManaMax = 0xCC78AC;            // Maximum Mana

        public int _Address_LocalPlayer_Exp = 0xCC7898;                // Current Experience
        public int _Address_LocalPlayer_ExpMax = 0xCC7888;             // Maximum Experience

        public int _Address_LocalPlayer_Level = 0xCC7880;              // Current Level

        public int _Address_LocalPlayer_HasTarget = 0x8774EC;          // Returns 1 if we have a target. 0 Otherwise

        public int _Address_LocalPlayer_Name = 0xD235A0;               // Our Name

        public int _Address_LocalPlayer_X = 0xCC17E8;                  // Player X Position
        public int _Address_LocalPlayer_Y = 0xCC17EC;                  // Player Y Position

        public int _Address_LocalPlayer_ClassID = 0xCC7918;            // 0 = Warrior. 1 = Gladiator

        //  Target
        //
        public int _Address_Target_Health = 0x8774E4;                  // Current Health
        public int[] offset_Target_Health = { 0x220, 0x11c8 };

        public int _Address_Target_HealthMax = 0x8774E4;               // Maximum Health
        public int[] offset_Target_HealthMax = { 0x220, 0x11cc };

        public int _Address_Target_Name = 0x8774E4;                    // Target Name string [20]
        public int[] offset_Target_Name = { 0x220, 0x3a };

        public int _Address_Target_Level = 0x8774E4;                   // Target Level
        public int[] offset_Target_Level = { 0x220, 0x36 };

        public int _Address_Target_Type = 0x8774E4;                    // Target Type
        public int[] offset_Target_Type = { 0x12c };

        public int _Address_Target_ID = 0x8774E4;
        public int[] offsets_Target_ID = { 0x220, 0x24 };

        public int _Address_Target_X = 0x8774E4;
        public int[] offsets_Target_X = { 0x34 };

        public int _Address_Target_Y = 0x8774E4;
        public int[] offsets_Target_Y = { 0x38 };
        

        // Camera
        //
        public int _Address_CameraX = 0xCBD964;
        public int _Address_CameraY = 0xCBD96C;
    };
}
