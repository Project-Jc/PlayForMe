using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MemoryEditor;
using MemoryAddresses;
using System.Windows.Forms;

namespace AionTools
{
    public class Tools
    {
        int _Address_Base;

        public float targetDistanceFromPlayer()
        {
            Memory oMemory = new Memory();
            MemoryLocation nmLocation = new MemoryLocation();

            if (oMemory.OpenProcess("aion.bin"))
            {
                _Address_Base = oMemory.BaseAddress("Game.dll");

                float X = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_X, nmLocation.offsets_Target_X);
                float Y = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Y, nmLocation.offsets_Target_Y);

                float distance = (float)Math.Sqrt(X * X + Y * Y);

                return distance;
            }
            else
            {
                MessageBox.Show("Couldn't find Aion... Check TargetDistance Code");
                
            }
            oMemory = null;
            nmLocation = null;
            return 0;
        }

        public bool PlayerHasTarget()
        {
            Memory oMemory = new Memory();
            MemoryLocation nmLocation = new MemoryLocation();

            if (oMemory.OpenProcess("aion.bin"))
            {
                if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HasTarget) == 1)
                {
                    return true;
                }
            }
            oMemory = null;
            nmLocation = null;
            return false;

        }

        public bool IsTargetDead()
        {
            Memory oMemory = new Memory();
            MemoryLocation nmLocation = new MemoryLocation();

            if (oMemory.OpenProcess("aion.bin"))
            {
                if (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Health, nmLocation.offset_Target_Health) == 0)
                {
                    return true;
                }
            }
            oMemory = null;
            nmLocation = null;
            return false;
        }

        public string TargetType()
        {

            Memory oMemory = new Memory();
            MemoryLocation nmLocation = new MemoryLocation();

            if (oMemory.OpenProcess("aion.bin"))
            {
                if (oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Type, nmLocation.offset_Target_Type, 4, false, 0) == "User")
                {
                    return "User";
                }
                if (oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Type, nmLocation.offset_Target_Type, 6, false, 0) == "Gather")
                {
                    return "Gather";
                }
                if (oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Type, nmLocation.offset_Target_Type, 3, false, 0) == "NPC")
                {
                    return "NPC";
                }

            }
            oMemory = null;
            nmLocation = null;
            return "";
        }



    }
}
