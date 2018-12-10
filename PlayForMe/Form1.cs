using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsInput;
using MemoryAddresses;
using MemoryEditor;
using MouseKeyboardLibrary;
using MS;

namespace Grievous
{
    public partial class Grievous : Form
    {
        // Initiate class instances
        Gladiator uGladiator = new Gladiator(); 
        Spiritmaster uSpiritmaster = new Spiritmaster(); 
        Chanter uChanter = new Chanter(); 
        Assassin uAssassin = new Assassin(); 
        Cleric uCleric = new Cleric(); 
        Ranger uRanger = new Ranger(); 
        Sorcerer uSorcerer = new Sorcerer();  
        
        // Declare memory instances
        MemoryLocation nmLocation = new MemoryLocation(); 
        Memory oMemory = new Memory(); 

        // Global process variables
        System.Diagnostics.Process[] AION; 
        int _Address_Base; 
        IntPtr child;
        int PID;

        // Global waypoint variables
        float[] WaypointsX = new float[200];  
        float[] WaypointsY = new float[200]; 
        short WaypointCount = 0; 
        short MoveCount = 0;
        float[] dWaypointsX = new float[200]; 
        float[] dWaypointsY = new float[200];
        short dWaypointCount = 0; 
        short dMoveCount = 0; 
        
        // Global bot/player variables         
        string CurrentClass;                    
        string[] Whispers = new string[5];      
        bool FinishedDeathRun;
        short pLevel; 
        short Levels; 
        short Kills;
        uint Exp; 
        short Stuck; 
        short Deaths; 
        short tRested; 
        short StayAbove;
        string[] BlackListName = new string[10]; 
        short BlackListCount = 0;

        // Buff variables
        int[] CoolDowns = new int[20];
        int[] BuffTime = new int[8] { 900, 900, 900, 60, 1800, 3600, 120, 0 }; 
        // For above - Buff1, Buff2, Buff3, One minute (Potions), Half an hour, One hour, Two minutes (Assassin poisons), bot start/stop
        
        // Automated quit flag
        bool AutomateQuitFLAG = false;
        
        public Grievous()
        {
            InitializeComponent();
        }
        private void PlayForMe_Load(object sender, EventArgs e)
        {
            // Declare the size of the main UI
            this.Size = new System.Drawing.Size(407, 620);

            // Search for AION processes
            AION = System.Diagnostics.Process.GetProcessesByName("aion.bin");
            if (AION.Length == 0) 
            {
                progLog(" Can't locate Aion");
                progLog(" Run Aion and reload application");
                Timer_Status.Stop();
                PID = 0;
                StatusPlayer_gBox.Text = string.Empty;
            }
            else 
            {
                EmbedAION.Enabled = true;
                progLog(" Located Aion process");
                for (int a = 0; a < AION.Length; a++)
                {
                    FindProcess_cBox.Items.Add(AION[a].Id);
                }
                FindProcess_cBox.SelectedIndex = 0;
            }
            // Hide default labels, combo boxes etc
            WaitingForInput.Text = string.Empty;
            Status_LocalPlayer_cmhpLabel.Text = string.Empty;
            Status_LocalPlayer_cmmpLabel.Text = string.Empty;
            Status_Target_cmhpLabel.Text = string.Empty;
            StatusTarget_gBox.Text = string.Empty;
            TargetRange_vLabel.Text = string.Empty;
            WaypointFile_Label.Text = string.Empty;
            ExpEarned_Label.Text = string.Empty;
            ElapsedTime.Hide();
            ExpEarned_Label.Hide();
            button1.Hide();
            StopLootingInventFull.Enabled = false;
            StopBottingAfterTime.Enabled = false;
            StopBottingAfterDeaths.Enabled = false;
            StopAfterMinutes.Enabled = false;
            StopAfterDeaths.Enabled = false;
            Logout.Enabled = false;
            UseReturn.Enabled = false;
            CraftBotType.Enabled = false;
            SafeRest.Enabled = false;
            SoulHealerNPC.Enabled = false;

            // Crappy machine locking code
            string JuliusID = "178BFBFF00100F43";
            System.Management.ManagementClass mClass = new System.Management.ManagementClass("win32_processor");
            System.Management.ManagementObjectCollection mOBJCOLLECTION = mClass.GetInstances();
            foreach (System.Management.ManagementObject mOBJ in mOBJCOLLECTION)
            {
                //MessageBox.Show(mOBJ.Properties["processorID"].Value.ToString());
                if (mOBJ.Properties["processorID"].Value.ToString() == JuliusID)
                {
                    //MessageBox.Show("Welcome Julius");
                }
                else
                {
                    progLog(" Illegal user detected");
                    progLog(" Locking program");
                    TabControl.Enabled = false;
                }
                break;
            }
        }

        // Timers
        private void Timer_Player_Target_HPMP_Tick(object sender, EventArgs e)
        {
            if (oMemory.OpenProcess(PID))
            {
                Status_LocalPlayer_cmhpLabel.Text = oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) + " / " + oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax);
                Status_LocalPlayer_cmmpLabel.Text = oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Mana) + " / " + oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_ManaMax);
                PlayerX_Label.Text = "Player X: " + oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X).ToString();
                PlayerY_Label.Text = "Player Y: " + oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y).ToString();
                if (PlayerHasTarget())
                {
                    TargetRange_vLabel.Text = TargetDistanceFromPlayer().ToString("0.0");
                    Distance_Label.Text = " Target range: " + TargetRange_vLabel.Text;
                    StatusTarget_gBox.Text = " " + oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Name, 40, true, 0);
                    TargetLevel_Label.Text = oMemory.ReadByte(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Level).ToString();
                    Status_Target_cmhpLabel.Text = oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) + " / " + oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax);
                }
                else
                {
                    StatusTarget_gBox.Text = string.Empty;
                    Status_Target_cmhpLabel.Text = string.Empty;
                    TargetLevel_Label.Text = string.Empty;
                    TargetRange_vLabel.Text = string.Empty;
                    Distance_Label.Text = string.Empty;
                }
            }
            else
            {
                Timer_Status.Stop();
            }
        }
        private void ToolTimer_Tick(object sender, EventArgs e)
        {
            if (oMemory.OpenProcess(PID))
            {
                BuffTime[3]++;
                BuffTime[4]++;
                BuffTime[5]++;
                BuffTime[6]++;
                /*
                if (System.Text.RegularExpressions.Regex.IsMatch(oMemory.ReadString(nmLocation._Chat_Whispers, 100, true), "[a-z0-9]", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                //if (!char.IsLetter(oMemory.ReadString(nmLocation._Chat_Whispers, 100, true).First()) && !char.IsWhiteSpace(oMemory.ReadString(nmLocation._Chat_Whispers, 100, true).First()))
                {
                    string[] PlayerName = oMemory.ReadString(nmLocation._Chat_Whispers, 100, true).Split(' ');
                    for (int i = 0; i < Whispers.Length; i++)
                    {
                        if (Whispers[i] == PlayerName[0])
                        {
                                                        // If we've already received a Whisper from this particular player
                                                        // Do nothing
                        }
                        else                                                // If we haven't previously received a Whisper from this player
                        {
                            Alert("Whisper");                               // Initiate appropriate user alerts
                            progLog(" Whisper from: " + PlayerName[0]);
                            Whispers[i] = PlayerName[0];                    // Add the Players name to the array
                            break;                                          // Break out of loop
                        }
                    } 
                } */
                if (StopBottingAfter.Checked && StopAfterMinutes.Checked)
                {
                    BuffTime[7]++;
                    if (StopBottingAfterTime.Text != string.Empty && BuffTime[7] >= Int32.Parse(StopBottingAfterTime.Text) * 60)
                    {
                        AutomateQuitFLAG = true;
                        LevelBot.CancelAsync();
                        BuffTime[7] = 0;
                    }
                }
                for (int i = 0; i < CoolDowns.Length; i++)
                {
                    CoolDowns[i]++;
                }
                if (!PlayerDead())
                {
                    if (Rest_UseHealthPotion.Checked)
                    {
                        if (KeymapGrid.Rows[4].Cells[1].Value != null && KeymapGrid.Rows[4].Cells[1].Value.ToString() != string.Empty)
                        {
                            if (ThirtySecondsCD.Checked)
                            {
                                if (BuffTime[3] >= 31 && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * Int32.Parse(Rest_UsePotionValue.Text) / 100)
                                {
                                    progLog(" Health below: " + Rest_UsePotionValue.Text + "% using HP potion");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[4].Cells[1].Value.ToString()), 0);
                                    BuffTime[3] = 0;
                                }
                            }
                            else
                            {
                                if (BuffTime[3] >= 61 && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * Int32.Parse(Rest_UsePotionValue.Text) / 100)
                                {
                                    progLog(" Health below: " + Rest_UsePotionValue.Text + "% using HP potion");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[4].Cells[1].Value.ToString()), 0);
                                    BuffTime[3] = 0;
                                }
                            }
                        }
                        else
                            progLog(" Error: No key set for HP potion");
                    }
                    else if (Rest_UseManaPotion.Checked)
                    {
                        if (KeymapGrid.Rows[4].Cells[1].Value != null && KeymapGrid.Rows[4].Cells[1].Value.ToString() != string.Empty)
                        {
                            if (ThirtySecondsCD.Checked)
                            {
                                if (BuffTime[3] >= 31 && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Mana) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_ManaMax) * Int32.Parse(Rest_UsePotionValue.Text) / 100)
                                {
                                    progLog(" Mana below " + Rest_UsePotionValue.Text + "% using MP potion");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[4].Cells[1].Value.ToString()), 0);
                                    BuffTime[3] = 0;
                                }
                            }
                            else
                            {
                                if (BuffTime[3] >= 61 && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Mana) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_ManaMax) * Int32.Parse(Rest_UsePotionValue.Text) / 100)
                                {
                                    progLog(" Mana below " + Rest_UsePotionValue.Text + "% using MP potion");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[4].Cells[1].Value.ToString()), 0);
                                    BuffTime[3] = 0;
                                }
                            }
                        }
                        else
                            progLog(" Error: No key set for MP potion");
                    }
                }
                if (FirstBuff_cBox.Checked)
                {
                    BuffTime[0]++;
                }
                if (SecondBuff_cbox.Checked)
                {
                    BuffTime[1]++;
                }
                if (ThirdBuff_cbox.Checked)
                {
                    BuffTime[2]++;
                }
            }
            else
            {
                ToolTimer.Stop();
            }
        }

        // Bot Worker
        private void Bot_BGWORK_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            bool DeathOverride = false;
            Rebuff(false);
            PlayerCheck(true);
            while (!bwAsync.CancellationPending)
            {
            // PLAYER DEAD LOOP
            PlayerDeadLoop:
                if (PlayerDead())
                {
                    Deaths += 1;
                    progLog(" Player died");
                    if (Boolean.Parse(BeepOnDeath.Checked.ToString()))
                    {
                        SoundAlert("PlayerDead");
                    }
                    if (!RevivePlayer())
                    {
                        progLog(" Failed to revive player");
                        goto Quit;
                    }
                    if (Boolean.Parse(StopBottingAfter.Checked.ToString()) && Boolean.Parse(StopAfterDeaths.Checked.ToString()))
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            if (StopBottingAfterDeaths.Text != string.Empty && StopBottingAfterDeaths.Text == Deaths.ToString())
                            {
                                DeathOverride = true;
                            }
                        }));
                    }
                    if (DeathOverride)
                    {
                        progLog(" Death limit reached!");
                        goto Quit;
                    }
                    if (dWaypointCount < 1)
                    {
                        progLog(" No death waypoints found");
                        goto Quit;
                    }
                    if (Boolean.Parse(SoulHealer.Checked.ToString()))
                    {
                        if (HealSoul())
                        {
                            progLog(" Recieved soul healing");
                        }
                        else
                        {
                            progLog(" Failed to receive soul healing");
                            Thread.Sleep(60000);
                        }
                    }
                    else
                        Thread.Sleep(60000);

                    // RESET DEBUFF COUNT
                    if (oMemory.OpenProcess(PID))
                        oMemory.Write(_Address_Base + nmLocation._Address_LocalPlayer_DebffCount, nmLocation.offsets_Debuffcount, 0);

                    dMoveCount = 0;
                    PlayerCheck(false);
                    Rebuff(true);
                    progLog(" Running to grinding spot");
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                    while (!FinishedDeathRun)
                    {
                        switch (MoveToHuntingArea())
                        {
                            case "Ok":
                                break;
                            case "Ambushed":
                                progLog(" Ambushed!");
                                if (EnterCombat(false, true) == "TargetDead")
                                {
                                    Loot();
                                    switch (PlayerCheck(false))
                                    {
                                        case "Ready":
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                                            break;
                                        case "Ambushed":
                                            goto case "Ambushed";
                                        case "PlayerDead":
                                            goto PlayerDeadLoop;
                                    }
                                    break;
                                }
                                else
                                    goto PlayerDeadLoop;
                        }
                        if (bwAsync.CancellationPending)
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            goto Quit;
                        }
                    }
                    progLog(" Arrived at grinding spot");
                    FinishedDeathRun = false;
                    MoveCount = 0;
                }
            // MAIN BOT LOOP
            MainLoop:
                while (!PlayerDead())
                {
                    if (bwAsync.CancellationPending)
                        goto Quit;
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.TAB, 0);
                    Thread.Sleep(50);
                    if (PlayerHasTarget() && TargetIsAppropriate())
                    {
                        switch (EnterCombat(false, false))
                        {
                            case "TargetDead":
                                Loot();
                                goto CheckPlayer;
                            case "TargetUnreachable":
                                if (ErrorScan() == "ObstacleBlocking")
                                    progLog(" Object blocking target");
                                else
                                    progLog(" Target unreachable");
                                while (PlayerHasTarget())
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                                    Thread.Sleep(500);
                                    if (PlayerDead())
                                        goto PlayerDeadLoop;
                                }
                                goto CheckPlayer;
                            case "PlayerDead":
                                goto PlayerDeadLoop;
                        }
                    }
                    else
                    {
                        switch (MoveToNextWaypoint())
                        {
                            case "Ok":
                                break;
                            case "Ambushed":
                                goto CheckPlayer;
                        }
                    }
                }
            // FOR PLAYER CHECKING
            CheckPlayer:
                if (bwAsync.CancellationPending)
                    goto Quit;
                switch (PlayerCheck(false))
                {
                    case "Ready":
                        switch (MoveToNextWaypoint())
                        {
                            case "Ok":
                                goto MainLoop;
                            case "Ambushed":
                                goto CheckPlayer;
                        }
                        break;
                    case "Ambushed":
                        progLog(" Ambushed!");
                        if (oMemory.OpenProcess(PID))
                        {
                            if (oMemory.ReadByte(_Address_Base + nmLocation._Address_AutoRunFlag) == 4)
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                        }
                        if (EnterCombat(false, true) == "TargetDead")
                        {
                            Loot();
                            goto CheckPlayer;
                        }
                        else
                            goto PlayerDeadLoop;

                    case "PlayerDead":
                        goto PlayerDeadLoop;
                }
            }
        Quit:
            this.BeginInvoke(new MethodInvoker(delegate
            {
                progLog(" Bot stopped");
                StartBot_btn.Text = "Stopped";
                FindProcess_cBox.Enabled = true;
                ReleaseAION.Enabled = true;
                ToolTimer.Stop();
                if (AutomateQuitFLAG)
                {
                    AutomateQuitFLAG = false;
                    if (UseReturn.Checked)
                    {
                        if (KeymapGrid.Rows[1].Cells[1].Value != null && KeymapGrid.Rows[1].Cells[1].Value.ToString() != string.Empty)
                        {
                            progLog(" Returning...");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[1].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(500);
                            do
                            {
                                Thread.Sleep(1000);
                            }
                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0);
                            Thread.Sleep(8000);
                        }
                        else
                            progLog(" Error: No key set for Return");
                    }
                    if (Logout.Checked)
                        QuitGame();

                    ReleaseAION.PerformClick();
                    PrintReport.PerformClick();
                }
            }));
            e.Cancel = true;
            return;
        }

        // Main UI form
        private void StartBot_btn_Click(object sender, EventArgs e)
        {
            /*
            if (CraftBox_.Checked)
            {
                if (!CraftBot.IsBusy)
                {
                    CraftBot.RunWorkerAsync();
                    StartBot_btn.Text = "Started";
                    progLog(" Craftbot Started");
                    return;
                }
                else
                {
                    CraftBot.CancelAsync();
                    StartBot_btn.Text = "Stopping";
                    return;
                }
            } */
            if (HealBot.Checked)
            {
                if (HealBotTarget.Text == null || HealBotTarget.Text == "")
                {
                    progLog(" Enter heal target first");
                    return;
                }
                if (!HealBot_BGWORK.IsBusy)
                {
                    HealBot_BGWORK.RunWorkerAsync();
                    StartBot_btn.Text = "Started";
                    progLog(" Healbot Started");
                    FindProcess_cBox.Enabled = false;
                    ToolTimer.Start();
                    return;
                }
                else
                {
                    HealBot_BGWORK.CancelAsync();
                    StartBot_btn.Text = "Stopping";
                    return;
                }
            }
            if (!LevelBot.IsBusy)
            {
                if (WaypointsX[0] == 0)
                {
                    progLog(" Error: No waypoints loaded");
                    return;
                }
                if (MaxTargetDistance.Text == String.Empty)
                {
                    progLog(" Error: Target distance unset");
                    return;
                }
                if (MaxTargetLevel.Text == String.Empty)
                {
                    progLog(" Error: Target level unset");
                    return;
                }
                short Count = 0;
                if (CurrentClass != "Starter")
                {
                    for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                    {
                        if (KeymapGrid.Rows[i].Cells[1].Value == null || KeymapGrid.Rows[i].Cells[1].Value.ToString() == String.Empty)
                        {
                            Count++;
                            if (Count >= KeymapGrid.Rows.Count - 5)
                            {
                                progLog(" Set keymap first!");
                                return;
                            }
                        }
                    }
                }
                switch (CurrentClass)
                {
                    case "Sorcerer":
                        if (uSorcerer.OpenSkill.SelectedIndex == -1)
                        {
                            progLog(" No opening skill selected");
                            progLog(" Defaulting to Ice Chain");
                            uSorcerer.OpenSkill.SelectedIndex = 0;
                        } break;
                    case "Ranger":
                        if (uRanger.OpenSkill.SelectedIndex == -1)
                        {
                            progLog(" No opening skill selected");
                            progLog(" Defaulting to Entangling Shot");
                            uAssassin.OpenSkill.SelectedIndex = 0;
                        } break;
                    case "Assassin":
                        if (uAssassin.OpenSkill.SelectedIndex == -1)
                        {
                            progLog(" No opening skill selected");
                            progLog(" Defaulting to Attack N Dash");
                            uAssassin.OpenSkill.SelectedIndex = 0;
                        } break;
                    case "Gladiator":
                        if (uGladiator.OpenSkill.SelectedIndex == -1)
                        {
                            progLog(" No opening skill selected");
                            progLog(" Defaulting to Attack");
                            uGladiator.OpenSkill.SelectedIndex = 0;
                        } break;
                    case "Spiritmaster":
                        if (uSpiritmaster.Sorcerer_OpenSkill_ddBox.SelectedIndex == -1)
                        {
                            progLog(" No opening skill selected");
                            return;
                        } break;
                    case "Chanter":
                        if (uChanter.OpenSkill.SelectedIndex == -1)
                        {
                            progLog(" No opening skill selected");
                            progLog(" Defaulting to Smite");
                            uChanter.OpenSkill.SelectedIndex = 0;
                        } break;
                    case "Cleric":
                        if (uCleric.OpenSkill.SelectedIndex == -1)
                        {
                            progLog(" No opening skill selected");
                            progLog(" Defaulting to Smite");
                            uCleric.OpenSkill.SelectedIndex = 0;
                        } break;
                }
                if (FindClosestWaypoint())
                {
                    movecount_Label.Text = "Current: " + MoveCount.ToString();
                    WaypointDisplay.SelectedIndex = MoveCount + 1;
                }
                else
                {
                    progLog(" Couldn't find a waypoint near enough");
                    // ADD FUNCTION TO TURN IN DIRECTION OF NEAREST WAYPOINT AND DISPLAY RELEVANT DISTANCE TO WAYPOINT
                    return;
                }
                if (StayAboveHealth.Text == string.Empty || StayAboveHealth.Text == "")
                {
                    //progLog(" Set StayAbove health first!");
                    StayAboveHealth.Text = "60";
                }
                // RESET BOT STOP AFTER TIME
                if (StopBottingAfter.Checked)
                {
                    if (StopBottingAfter.Text == string.Empty)
                    {
                        progLog(" Error: No time limit set");
                        return;
                    }
                    progLog(" Bot stop time set for " + DateTime.Now.AddMinutes(Double.Parse(StopBottingAfterTime.Text)).ToLongTimeString());
                }
                BuffTime[7] = 0;
                // RESET COOLDOWN TIMES
                for (int dC = 0; dC < CoolDowns.Length; dC++)
                {
                    CoolDowns[dC] = 1000;
                }
                // EMBED AION CLIENT
                if (EmbedAION.Enabled)
                {
                    EmbedAION.PerformClick();
                }
                ReleaseAION.Enabled = false;
                FindProcess_cBox.Enabled = false;
                ToolTimer.Start();
                LevelBot.RunWorkerAsync();
                StartBot_btn.Text = "Started";
                progLog(" Bot started");
            }
            else
            {
                LevelBot.CancelAsync();
                StartBot_btn.Text = "Stopping";
            }
        }
        private void FindProcess_cBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (FindProcess_cBox.Text == PID.ToString())
                return;

            PID = AION[FindProcess_cBox.SelectedIndex].Id;
            child = System.Diagnostics.Process.GetProcessById(PID).MainWindowHandle;

            if (oMemory.OpenProcess(PID))
            {
                _Address_Base = oMemory.BaseAddress("Game.dll");
                String PlayerName = null;
                foreach (char c in oMemory.ReadString(_Address_Base + nmLocation._Address_LocalPlayer_Name, 20, true))
                {
                    if (char.IsLetter(c)) 
                        PlayerName += c; 
                    else 
                        continue;
                }
                if (PlayerName == string.Empty || PlayerName == null || PlayerName == "")
                {
                    progLog(" Character must be logged in");
                    progLog(" Login before continuing... ");
                    StatusPlayer_gBox.Text = string.Empty;
                    this.Text = "Grievous";
                    return;
                }
                this.Text = "Grievous - " + PlayerName;
                StatusPlayer_gBox.Text = PlayerName;
                Exp = oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Exp);
                // Set keymap path
                String keyPath = Application.StartupPath + "\\" + PlayerName + "\\keymap.txt";
                // Set config path
                String cfgPath = Application.StartupPath + "\\" + PlayerName + "\\" + PlayerName + ".cfg";
                // Remove pre-existing datagridrows to allow for new ones
                KeymapGrid.RowCount = 0;
                switch (oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_ClassID))
                {
                    case 0:
                        // GLADIATOR-TEMPLAR BASE CLASS
                        label1.Hide();
                        Rest_Health.Text = "50";
                        Rest_Mana.Text = "50";
                        progLog(" Starter class loaded");
                        CurrentClass = "Starter";
                        ClassLabel.Text = CurrentClass;
                        this.Text += " - [" + CurrentClass.ToUpper() + "]";
                        String[] gtActions = {
                                                   "Attack",
                                                   "Return",
                                                   "Loot",
                                                   "HerbTreatment",
                                                   "Potion",
                                                   "Skill", 
                                            };
                        KeymapGrid.Rows.Add(gtActions.Length);
                        for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                        {
                            KeymapGrid.Rows[i].Cells[0].Value = gtActions[i];
                        }
                        return;
                    case 1:
                        progLog(" Gladiator class loaded");
                        CurrentClass = "Gladiator";
                        ClassLabel.Text = CurrentClass;
                        this.Text += " - [" + CurrentClass.ToUpper() + "]";
                        // LOAD GLADIATOR CONFIG
                        if (System.IO.File.Exists(cfgPath))
                        {
                            progLog(" " + PlayerName + " config found!");
                            foreach (String CurrentLine in System.IO.File.ReadAllLines(cfgPath))
                            {
                                if (CurrentLine == "[GLADIATOR]")
                                    continue;
                                if (CurrentLine == "[GENERAL]")
                                    break;
                                String[] SplitString = CurrentLine.Split(' ');
                                if (SplitString[0] == "OpenSkill")
                                    uGladiator.OpenSkill.SelectedIndex = Int32.Parse(SplitString[1]);
                                foreach (Control GladiatorControl in uGladiator.Controls)
                                {
                                    if (SplitString[0] == GladiatorControl.Name)
                                    {
                                        if (GladiatorControl is CheckBox)
                                        {
                                            CheckBox GladiatorCheck = GladiatorControl as CheckBox;
                                            GladiatorCheck.Checked = bool.Parse(SplitString[1]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            progLog(" " + PlayerName + " config not found");
                            Rest_Health.Text = "50";
                            Rest_Mana.Text = "50";
                        }
                        // CREATE GLADIATOR KEYMAP IF NOT FOUND
                        if (!System.IO.File.Exists(keyPath))
                        {
                            progLog(" No keymap found");
                            String[] gActions = {
                                                   "Attack",
                                                   "Return",
                                                   "Loot",
                                                   "HerbTreatment",
                                                   "Potion",
                                                   "Buff1",
                                                   "Buff2",
                                                   "Buff3",
                                                   "AionsStrength",
                                                   "DefencePrep",
                                                   "ImprovedStamina",
                                                   "SevereBlow",
                                                   "FerociousStrike",
                                                   "Cleave",
                                                   "BodySmash",
                                                   "SeismicWave",
                                                   "WeakeningBlow",
                                                   "PiercingWave",
                                                   "AerialLockdown",
                                                   "Taunt",
                                                   "StaminaRecovery",
                                                   "UnwaveringDevotion",
                                                   "WallofSteel",
                                                   "DaevicFury",    
                                               };
                            KeymapGrid.Rows.Add(gActions.Length);
                            for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                            {
                                KeymapGrid.Rows[i].Cells[0].Value = gActions[i];
                            }
                        }
                        else
                            // LOAD GLADIATOR KEYMAP IF FOUND
                            goto LoadKeyMap;
                        break;
                    case 3:
                        // ASSASSIN-RANGER BASE CLASS
                        Rest_Health.Text = "50";
                        Rest_Mana.Text = "50";
                        progLog(" Starter class loaded");
                        CurrentClass = "Starter";
                        ClassLabel.Text = CurrentClass;
                        this.Text += " - [" + CurrentClass.ToUpper() + "]";
                        String[] arActions = {
                                                   "Attack",
                                                   "Return",
                                                   "Loot",
                                                   "HerbTreatment",
                                                   "Potion",
                                                   "Skill", 
                                            };
                        KeymapGrid.Rows.Add(arActions.Length);
                        for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                        {
                            KeymapGrid.Rows[i].Cells[0].Value = arActions[i];
                        }
                        return;
                    case 4:
                        progLog(" Assassin class loaded");
                        CurrentClass = "Assassin";
                        ClassLabel.Text = CurrentClass;
                        this.Text += " - [" + CurrentClass.ToUpper() + "]";
                        // LOAD ASSASSIN CONFIG IF FOUND
                        if (System.IO.File.Exists(cfgPath))
                        {
                            progLog(" " + PlayerName + " config found!");
                            foreach (String CurrentLine in System.IO.File.ReadAllLines(cfgPath))
                            {
                                if (CurrentLine == "[ASSASSIN]")
                                    continue;
                                if (CurrentLine == "[GENERAL]")
                                    break;
                                String[] SplitString = CurrentLine.Split(' ');
                                if (SplitString[0] == "OpenSkill")
                                    uAssassin.OpenSkill.SelectedIndex = Int32.Parse(SplitString[1]);
                                foreach (Control AssassinControl in uAssassin.Controls)
                                {
                                    if (SplitString[0] == AssassinControl.Name)
                                    {
                                        if (AssassinControl is CheckBox)
                                        {
                                            CheckBox AssassinCheck = AssassinControl as CheckBox;
                                            AssassinCheck.Checked = bool.Parse(SplitString[1]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            progLog(" " + PlayerName + " config not found");
                            Rest_Health.Text = "50";
                            Rest_Mana.Text = "50";
                        }
                        // CREATE ASSASSIN KEYMAP IF NOT FOUND
                        if (!System.IO.File.Exists(keyPath))
                        {
                            progLog(" No keymap found");
                            String[] aActions = {
                                                   "Attack",
                                                   "Return",
                                                   "Loot",
                                                   "HerbTreatment",
                                                   "Potion",
                                                   "Buff1",
                                                   "Buff2",
                                                   "Buff3",
                                                   "SwiftEdge",
                                                   "FocusedEvasion",
                                                   "SurpriseAttack",
                                                   "FangStrike",
                                                   "RuneCarve",
                                                   "PainRune",
                                                   "Devotion",
                                                   "ApplyPoison",
                                                   "DashAttack",
                                                   "ClearFocus",
                                                   "BloodRune",
                                                   "Ambush",
                                                   "Sprint",
                                                   "RipclawStrike",
                                                   "WeakeningBlow",
                                                   "DarknessRune",
                                                   "KillersEye",
                                                   "Flurry",
                                               };
                            KeymapGrid.Rows.Add(aActions.Length);
                            for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                            {
                                KeymapGrid.Rows[i].Cells[0].Value = aActions[i];
                            }
                        }
                        else
                            // LOAD ASSASSIN KEYMAP IF FOUND
                            goto LoadKeyMap;
                        break;
                    case 5:
                        progLog(" Ranger class loaded");
                        CurrentClass = "Ranger";
                        ClassLabel.Text = CurrentClass;
                        this.Text += " - [" + CurrentClass.ToUpper() + "]";
                        // LOAD RANGER CONFIG IF FOUND
                        if (System.IO.File.Exists(cfgPath))
                        {
                            progLog(" " + PlayerName + " config found!");
                            foreach (String CurrentLine in System.IO.File.ReadAllLines(cfgPath))
                            {
                                if (CurrentLine == "[RANGER]")
                                    continue;
                                if (CurrentLine == "[GENERAL]")
                                    break;
                                String[] SplitString = CurrentLine.Split(' ');
                                if (SplitString[0] == "OpenSkill")
                                    uRanger.OpenSkill.SelectedIndex = Int32.Parse(SplitString[1]);
                                foreach (Control RangerControl in uRanger.Controls)
                                {
                                    if (SplitString[0] == RangerControl.Name)
                                    {
                                        if (RangerControl is CheckBox)
                                        {
                                            CheckBox RangerCheck = RangerControl as CheckBox;
                                            RangerCheck.Checked = bool.Parse(SplitString[1]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            progLog(" " + PlayerName + " config not found");
                            Rest_Health.Text = "50";
                            Rest_Mana.Text = "50";
                        }
                        // CREATE RANGER KEYMAP IF NOT FOUND
                        if (!System.IO.File.Exists(keyPath))
                        {
                            progLog(" No keymap found");
                            String[] rActions = {
                                                   "Attack",
                                                   "Return",
                                                   "Loot",
                                                   "HerbTreatment",
                                                   "Potion",
                                                   "Buff1",
                                                   "Buff2",
                                                   "Buff3",
                                                   "EntanglingShot",
                                                   "SwiftShot",
                                                   "Devotion",
                                                   "FocusedEvasion",
                                                   "Dodging",
                                                   "Deadshot",
                                                   "Aiming",
                                                   "StunningShot",
                                                   "StrongShots",
                                                   "MauForm",
                                               };
                            KeymapGrid.Rows.Add(rActions.Length);
                            for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                            {
                                KeymapGrid.Rows[i].Cells[0].Value = rActions[i];
                            }
                        }
                        else
                            // LOAD RANGER KEYMAP IF FOUND
                            goto LoadKeyMap;
                        break;
                    case 6:
                        // SPIRITMASTER - SORCERER BASE CLASS
                        Rest_Health.Text = "50";
                        Rest_Mana.Text = "50";
                        progLog(" Starter class loaded");
                        CurrentClass = "Starter";
                        ClassLabel.Text = CurrentClass;
                        this.Text += " - [" + CurrentClass.ToUpper() + "]";
                        String[] maActions = {
                                                   "Attack",
                                                   "Return",
                                                   "Loot",
                                                   "HerbTreatment",
                                                   "Potion",
                                                   "Skill", 
                                            };
                        KeymapGrid.Rows.Add(maActions.Length);
                        for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                        {
                            KeymapGrid.Rows[i].Cells[0].Value = maActions[i];
                        }
                        return;
                    case 7:
                        progLog(" Sorcerer class loaded");
                        CurrentClass = "Sorcerer";
                        ClassLabel.Text = CurrentClass;
                        this.Text += " - [" + CurrentClass.ToUpper() + "]";
                        // LOAD SORCERER CONFIG IF FOUND
                        if (System.IO.File.Exists(cfgPath))
                        {
                            progLog(" " + PlayerName + " config found!");
                            foreach (String CurrentLine in System.IO.File.ReadAllLines(cfgPath))
                            {
                                if (CurrentLine == "[SORCERER]")
                                    continue;
                                if (CurrentLine == "[GENERAL]")
                                    break;
                                String[] SplitString = CurrentLine.Split(' ');
                                if (SplitString[0] == "OpenSkill")
                                    uSorcerer.OpenSkill.SelectedIndex = Int32.Parse(SplitString[1]);
                                foreach (Control SorcererControl in uSorcerer.Controls)
                                {
                                    if (SplitString[0] == SorcererControl.Name)
                                    {
                                        if (SorcererControl is CheckBox)
                                        {
                                            CheckBox SorcererCheck = SorcererControl as CheckBox;
                                            SorcererCheck.Checked = bool.Parse(SplitString[1]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            progLog(" " + PlayerName + " config not found");
                            Rest_Health.Text = "50";
                            Rest_Mana.Text = "50";
                        }
                        // CREATE SORCERER KEYMAP IF FOUND
                        if (!System.IO.File.Exists(keyPath))
                        {
                            progLog(" No keymap found");
                            String[] sActions = {
                                                   "Attack",
                                                   "Return",
                                                   "Loot",
                                                   "HerbTreatment",
                                                   "Potion",
                                                   "Buff1",
                                                   "Buff2",
                                                   "Buff3",
                                                   "IceChain",
                                                   "FlameBolt",
                                                   "Erosion",
                                                   "Root",
                                                   "AbsorbEnergy",
                                                   "StoneSkin",
                                                   "RobeBuff",
                                                   "FlameHarpoon",
                                                   "BlindLeap",
                                                   "FlameCage",
                                                   "WinterBinding",
                                                   "DelayedBlast",
                                                   "FreezingWind",
                                                   "GainMana",
                                                   "FlameFusion",
                                               };
                            KeymapGrid.Rows.Add(sActions.Length);
                            for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                            {
                                KeymapGrid.Rows[i].Cells[0].Value = sActions[i];
                            }
                        }
                        else
                            // LOAD SORCERER KEYMAP IF FOUND
                            goto LoadKeyMap;
                        break;
                    case 9:
                        // CHANTER-CLERIC BASE CLASS
                        Rest_Health.Text = "50";
                        Rest_Mana.Text = "50";
                        progLog(" Starter class loaded");
                        CurrentClass = "Starter";
                        ClassLabel.Text = CurrentClass;
                        this.Text += " - [" + CurrentClass.ToUpper() + "]";
                        String[] bActions = {
                                                   "Attack",
                                                   "Return",
                                                   "Loot",
                                                   "HerbTreatment",
                                                   "Potion",
                                                   "Skill",   
                                            };
                        KeymapGrid.Rows.Add(bActions.Length);
                        for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                        {
                            KeymapGrid.Rows[i].Cells[0].Value = bActions[i];
                        }
                        return;
                    case 10:
                        // CLERIC
                        progLog(" Cleric class loaded");
                        CurrentClass = "Cleric";
                        ClassLabel.Text = CurrentClass;
                        this.Text += " - [" + CurrentClass.ToUpper() + "]";
                        if (System.IO.File.Exists(cfgPath))
                        {
                            progLog(" " + PlayerName + " config found!");
                            foreach (String CurrentLine in System.IO.File.ReadAllLines(cfgPath))
                            {
                                if (CurrentLine == "[ClERIC]")
                                    continue;
                                if (CurrentLine == "[GENERAL]")
                                    break;
                                String[] SplitString = CurrentLine.Split(' ');
                                if (SplitString[0] == "OpenSkill")
                                    uCleric.OpenSkill.SelectedIndex = Int32.Parse(SplitString[1]);
                                foreach (Control ClericControl in uCleric.Controls)
                                {
                                    if (SplitString[0] == ClericControl.Name)
                                    {
                                        if (ClericControl is CheckBox)
                                        {
                                            CheckBox ClericCheck = ClericControl as CheckBox;
                                            ClericCheck.Checked = bool.Parse(SplitString[1]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            progLog(" " + PlayerName + " config not found");
                            Rest_Health.Text = "50";
                            Rest_Mana.Text = "50";
                            StayAboveHealth.Text = "50";
                        }
                        // CREATE CLERIC KEYMAP IF FOUND
                        if (!System.IO.File.Exists(keyPath))
                        {
                            progLog(" No keymap found");
                            String[] cLActions = {
                                                   "Attack",
                                                   "Return",
                                                   "Loot",
                                                   "HerbTreatment",
                                                   "Potion",
                                                   "Buff1",
                                                   "Buff2",
                                                   "Buff3",
                                                   "PromiseofWind",
                                                   "BlessingofRock",
                                                   "BlessingofHealth",
                                                   "HallowedStrike",  
                                                   "Smite",
                                                   "HealingLight",
                                                   "LightofRenewal",
                                                   "SummonHolyServant",
                                                   "StormofAion",
                                                   "Dispel",
                                                   "ElementCircle",
                                                   "LightofRejuvenation",
                                                   "Penance",
                                                   "Chastise",
                                                   "Root",
                                                   "RadiantCure",
                                                   "BlessedShield",
                                                   "PunishingWind",
                                               };
                            KeymapGrid.Rows.Add(cLActions.Length);
                            for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                            {
                                KeymapGrid.Rows[i].Cells[0].Value = cLActions[i];
                            }
                        }
                        else
                            goto LoadKeyMap;
                        break;
                    case 11:
                        // CHANTER
                        progLog(" Chanter class loaded");
                        CurrentClass = "Chanter";
                        ClassLabel.Text = CurrentClass;
                        this.Text += " - [" + CurrentClass.ToUpper() + "]";
                        if (System.IO.File.Exists(cfgPath))
                        {
                            progLog(" " + PlayerName + " config found!");
                            foreach (String CurrentLine in System.IO.File.ReadAllLines(cfgPath))
                            {
                                if (CurrentLine == "[CHANTER]")
                                    continue;
                                if (CurrentLine == "[GENERAL]")
                                    break;
                                String[] SplitString = CurrentLine.Split(' ');
                                if (SplitString[0] == "OpenSkill")
                                    uChanter.OpenSkill.SelectedIndex = Int32.Parse(SplitString[1]);

                                foreach (Control ChanterControl in uChanter.Controls)
                                {
                                    if (SplitString[0] == ChanterControl.Name)
                                    {
                                        if (ChanterControl is CheckBox)
                                        {
                                            CheckBox ChanterCheck = ChanterControl as CheckBox;
                                            ChanterCheck.Checked = bool.Parse(SplitString[1]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            progLog(" " + PlayerName + " config not found");
                            Rest_Health.Text = "50";
                            Rest_Mana.Text = "50";
                            StayAboveHealth.Text = "50";
                        }
                        // CREATE CHANTER KEYMAP IF NOT FOUND
                        if (!System.IO.File.Exists(keyPath))
                        {
                            progLog(" No keymap found");
                            String[] chActions = {
                                                   "Attack",
                                                   "Return",
                                                   "Loot",
                                                   "HerbTreatment",
                                                   "Potion",
                                                   "Buff1",
                                                   "Buff2",
                                                   "Buff3",
                                                   "RageSpell",
                                                   "PromiseofWind",
                                                   "BlessingofRock",
                                                   "BlessingofHealth",
                                                   "Mantra1",
                                                   "Mantra2",
                                                   "Mantra3",
                                                   "ProtectiveWard",
                                                   "WordofQuickness",
                                                   "HallowedStrike",
                                                   "MeteorStrike",
                                                   "SoulStrike",
                                                   "HealingLight",
                                                   "RecoverySpell",
                                                   "WordofRevival",
                                                   "FocusedParry",
                                                   "PerfectParry",
                                                   "AethericField",
                                                   "Smite",
                                                   "WordofWind",  
                                                   "SpellofAscension",
                                                   "Annihilation",
                                               };

                            KeymapGrid.Rows.Add(chActions.Length);
                            for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                            {
                                KeymapGrid.Rows[i].Cells[0].Value = chActions[i];
                            }
                        }
                        else
                            // LOAD CHANTER KEYMAP IF FOUND
                            goto LoadKeyMap;
                        break;
                }
      LoadKeyMap:
                // LOAD FOUND KEYMAP
                if (System.IO.File.Exists(keyPath))
                {
                    progLog(" Keymap found!");
                    int index = 0;
                    foreach (String CurrentLine in System.IO.File.ReadAllLines(keyPath))
                    {
                        if (CurrentLine != string.Empty)
                        {
                            KeymapGrid.Rows.Add(1);
                            String[] Ex = CurrentLine.Split(' ');
                            KeymapGrid.Rows[index].Cells[0].Value = Ex[0];
                            if (!string.IsNullOrWhiteSpace(Ex[1]))
                            {
                                KeymapGrid.Rows[index].Cells[1].Value = Ex[1];
                                index++;
                            }
                            else
                                index++;
                        }
                    }
                }
                // LOAD CFG FILE
                if (System.IO.File.Exists(cfgPath))
                {
                    foreach (String CurrentLine in System.IO.File.ReadAllLines(cfgPath))
                    {
                        String[] SplitString = CurrentLine.Split(' ');

                        if (SplitString[0] == MinimizeMode.Name) MinimizeMode.Checked = bool.Parse(SplitString[1]);
                        if (SplitString[0] == StealMobs.Name) StealMobs.Checked = bool.Parse(SplitString[1]);
                        if (SplitString[0] == ChaseRunners.Name) ChaseRunners.Checked = bool.Parse(SplitString[1]);
                        if (SplitString[0] == BeepOnKill.Name) BeepOnKill.Checked = bool.Parse(SplitString[1]);
                        if (SplitString[0] == BeepOnDeath.Name) BeepOnDeath.Checked = bool.Parse(SplitString[1]);
                        if (SplitString[0] == SkipLoot.Name) SkipLoot.Checked = bool.Parse(SplitString[1]);
                        if (SplitString[0] == StopLootingInventFull.Name) StopLootingInventFull.Checked = bool.Parse(SplitString[1]);

                        if (SplitString[0] == Rest_Health.Name) Rest_Health.Text = SplitString[1];
                        if (SplitString[0] == Rest_Mana.Name) Rest_Mana.Text = SplitString[1];
                        if (SplitString[0] == Rest_UseHerbTreatment.Name) Rest_UseHerbTreatment.Checked = bool.Parse(SplitString[1]);
                        if (SplitString[0] == Rest_UseHerbTreatmentValue.Name) Rest_UseHerbTreatmentValue.Text = SplitString[1];
                        if (SplitString[0] == Rest_UseHealthPotion.Name) Rest_UseHealthPotion.Checked = bool.Parse(SplitString[1]);
                        if (SplitString[0] == Rest_UseManaPotion.Name) Rest_UseManaPotion.Checked = bool.Parse(SplitString[1]);
                        if (SplitString[0] == Rest_UsePotionValue.Name) Rest_UsePotionValue.Text = SplitString[1];
                        if (SplitString[0] == FirstBuff_Duration.Name) FirstBuff_Duration.Text = SplitString[1];
                        if (SplitString[0] == SecondBuff_Duration.Name) SecondBuff_Duration.Text = SplitString[1];
                        if (SplitString[0] == ThirdBuff_Duration.Name) ThirdBuff_Duration.Text = SplitString[1];

                        if (SplitString[0] == StayAboveHealth.Name) StayAboveHealth.Text = SplitString[1];
                    }
                }
                if (CurrentClass == "Cleric" || CurrentClass == "Chanter")
                {
                    HealBot.Enabled = true;
                    HealBotTarget.Enabled = true;
                    StayAboveHealth.Show();
                    label2.Show();
                    label1.Show();
                }
                else
                {
                    HealBot.Enabled = false;
                    HealBotTarget.Enabled = false;
                    StayAboveHealth.Hide();
                    label2.Hide();
                    label1.Hide();
                }
            }
        }
        /*
        private void CraftBot_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            Rectangle rect;
            Boolean Completed = false; 
            short MouseDelay = 250;
            IntPtr fgWnd;
            String Type = null; this.BeginInvoke(new MethodInvoker(delegate { Type = CraftBotType.Text; }));
            while (!bwAsync.CancellationPending)
            {
                if (Type == "Cooking")
                {
                    if (oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Name, 14, true, 0) == "Lainita")
                    {
                        if (Completed)
                        {
                            fgWnd = GetForegroundWindow();
                            if (GetForegroundWindow() == child)
                            {
                                this.BeginInvoke(new MethodInvoker(delegate { progLog(" Opening dialogue window "); }));
                                Thread.Sleep(MouseDelay);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);
                                Thread.Sleep(MouseDelay);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);
                                Thread.Sleep(MouseDelay);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);

                                this.BeginInvoke(new MethodInvoker(delegate { progLog(" Selecting work order "); }));
                                GetWindowRect(child, out rect);
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.X = rect.X + 125; MouseSimulator.Y = rect.Y + 310;   // Work order select
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.Click(MouseButton.Left);
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.Y = rect.Y + 210;                                    // Select quest                                   
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.Click(MouseButton.Left);
                                Thread.Sleep(MouseDelay);

                                this.BeginInvoke(new MethodInvoker(delegate { progLog(" Returning work order "); }));
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.X = rect.X + 190; MouseSimulator.Y = rect.Y + 337;   // Accept button
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.Click(MouseButton.Left);
                                Completed = false;
                                SetForegroundWindow(fgWnd);
                            }
                            else SetForegroundWindow(child);
                        }
                        else
                        {
                            fgWnd = GetForegroundWindow();
                            if (GetForegroundWindow() == child)
                            {
                                this.BeginInvoke(new MethodInvoker(delegate { progLog(" Opening dialogue window "); }));
                                Thread.Sleep(MouseDelay);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);
                                Thread.Sleep(MouseDelay);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);
                                Thread.Sleep(MouseDelay);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);

                                this.BeginInvoke(new MethodInvoker(delegate { progLog(" Selecting work order "); }));
                                GetWindowRect(child, out rect);
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.X = rect.X + 125; MouseSimulator.Y = rect.Y + 310;   // Work order select
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.Click(MouseButton.Left);
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.Y = rect.Y + 210;                                    // Select quest                                   
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.Click(MouseButton.Left);

                                this.BeginInvoke(new MethodInvoker(delegate { progLog(" Accepting... "); }));
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.X = rect.X + 190; MouseSimulator.Y = rect.Y + 337;   // Accept button
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.Click(MouseButton.Left);

                                this.BeginInvoke(new MethodInvoker(delegate { progLog(" Selecting craft item "); }));
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.X = rect.X + 550; MouseSimulator.Y = rect.Y + 130;   // Relevant selection in craft window
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.Click(MouseButton.Left);
                                Thread.Sleep(MouseDelay);

                                this.BeginInvoke(new MethodInvoker(delegate { progLog(" Crafting all "); }));
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.X = rect.X + 650; MouseSimulator.Y = rect.Y + 380;   // Craft all button
                                Thread.Sleep(MouseDelay);
                                MouseSimulator.Click(MouseButton.Left);
                                for (int i = 0; i < 180; i++)
                                {
                                    Thread.Sleep(1000);
                                    if (bwAsync.CancellationPending)
                                    {
                                        this.BeginInvoke(new MethodInvoker(delegate { StartBot_btn.Text = "Stopped"; progLog(" Craftbot stopped"); }));
                                        e.Cancel = true;
                                        return;
                                    }
                                }
                                SetForegroundWindow(fgWnd);
                                Completed = true;
                            }
                            else SetForegroundWindow(child);
                        }
                    }
                    else
                    {
                        //SelectTargetByName("Lainita");
                        Thread.Sleep(1000);
                    }
                }
            }
        }
        */
        private void EmbedAION_Click(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(1212, 677);
            SetParent(child, this.AionPanel.Handle);
            ShowWindowAsync(child, 3);
            EmbedAION.Enabled = false;
            ReleaseAION.Enabled = true;
            //long WindowStyle = GetWindowLong(child, -16);
            /*
            const Int32 GWL_STYLE = (-16);
            const Int32 WS_CAPTION = 0xC00000;
            const Int32 WS_THICKFRAME = 0x40000;
            const Int32 WS_SIZEBOX = WS_THICKFRAME;
            const Int32 WS_DLGFRAME = 0x400000;
            const Int32 WS_BORDER = 0x800000;

            int lStyle = GetWindowLong(child, GWL_STYLE);
            lStyle &= ~(WS_CAPTION | WS_BORDER | WS_DLGFRAME | WS_SIZEBOX | WS_THICKFRAME);
           
            SetWindowLong(child, GWL_STYLE, lStyle);
            
            const int GWL_STYLE = -16;              //hex constant for style changing
            const int WS_BORDER = 0x00800000;       //window with border
            const int WS_CAPTION = 0x00C00000;      //window with a title bar
            const int WS_SYSMENU = 0x00080000;      //window with no borders etc.
            const int WS_MINIMIZEBOX = 0x00020000;  //window with minimizebox
            SetWindowLong(child, GWL_STYLE, WS_SYSMENU);
            */
            Thread.Sleep(2000);
        }
        private void HealBot_BGWORK_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bwAsync = sender as BackgroundWorker;
            if (oMemory.OpenProcess(PID))
            {
                while (!bwAsync.CancellationPending)
                {
                    // IF PLAYER DEAD
                    if (PlayerDead())
                    {
                        this.BeginInvoke(new MethodInvoker(delegate { StartBot_btn.Text = "Stopped"; progLog(" Healbot stopped"); }));
                        e.Cancel = true;
                        return;
                        // END THE BOT
                    }
                    // IF WE'RE NOT TARGETING OUR HEALING TARGET
                    if (!TargetIsHealingTarget())
                    {
                        //progLog(" Targeting healing target");
                        // TARGET IT
                        SelectTargetByName(HealBotTarget.Text);
                    }
                    if (TargetDistanceFromPlayer() >= 6)
                    {
                        // IF WE'RE NOT FOLLOWING HEAL TARGET
                        if (oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_Following) == 0)
                        {
                            //progLog(" Following target");
                            FollowTarget();
                        }
                    }
                    // IF OUR TARGET NEEDS HEALING
                    if (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax) * 80 / 100)
                    {
                        // BEGIN HEAL UNTIL SATISFIED
                        progLog(" Healing target");
                        if (CurrentClass == "Cleric")
                        {
                            while (!PlayerDead() && oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax) * 90 / 100)
                            {
                                if (KeymapGrid.Rows[23].Cells[1].Value != null && KeymapGrid.Rows[23].Cells[1].Value.ToString() != string.Empty)
                                {
                                    if (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax) * 60 / 100)
                                    {
                                        progLog(" Radiant Cure");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[23].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            //progLog(" Casting detected");
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                            {
                                                Thread.Sleep(100);
                                            }
                                            Thread.Sleep(1400);
                                        }
                                    }
                                }
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(500);
                                if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                {
                                    //progLog(" Casting detected");
                                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    Thread.Sleep(1400);
                                }
                                if (Boolean.Parse(uCleric.UseLightofRenewal.Checked.ToString()))
                                {
                                    if (CoolDowns[1] >= 10)
                                    {
                                        CoolDowns[1] = 0;
                                        //progLog(" Light of Renewal");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                {
                                    if (KeymapGrid.Rows[19].Cells[1].Value != null && KeymapGrid.Rows[19].Cells[1].Value.ToString() != string.Empty)
                                    {
                                        if (CoolDowns[1] >= 31)
                                        {
                                            CoolDowns[1] = 0;
                                            //progLog(" Light of Rejuvenation");
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(1400);
                                        }
                                    }
                                    else
                                        progLog(" Error: No key set for Light of Rejuvenation");
                                }
                            }
                        }                              
                    }
                    // IF OUR HEALTH IS GETTING LOW
                    if (PlayerHealthBelowPercent(Int32.Parse(StayAboveHealth.Text)))
                    {
                        progLog(" Deselecting target");
                        progLog(" Healing self");
                        // DESLECT CURRENT TARGET
                        do
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                            Thread.Sleep(500);
                        }
                        while (!PlayerDead() && PlayerHasTarget());
                        // BEGIN HEALING MYSELF
                        while (!PlayerDead() && PlayerHealthBelowPercent(Int32.Parse(StayAboveHealth.Text)))
                        {
                            if (KeymapGrid.Rows[24].Cells[1].Value != null && KeymapGrid.Rows[24].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (CoolDowns[7] >= 121)
                                {
                                    CoolDowns[7] = 0;
                                    //progLog(" Blessed Shield");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[24].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1400);
                                }
                            }
                            if (KeymapGrid.Rows[23].Cells[1].Value != null && KeymapGrid.Rows[23].Cells[1].Value.ToString() != string.Empty)
                            {
                                int RadiantPercent;
                                if (StayAbove - 30 <= 0)
                                    RadiantPercent = 35;
                                else
                                    RadiantPercent = StayAbove - 35;

                                if (PlayerHealthBelowPercent(RadiantPercent))
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[23].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                    {
                                        //progLog(" Casting detected");
                                        while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            Thread.Sleep(100);
                                        }
                                        Thread.Sleep(1400);
                                    }
                                }
                            }
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(500);
                            if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                            {
                                //progLog(" Casting detected");
                                while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                {
                                    Thread.Sleep(100);
                                }
                                Thread.Sleep(1400);
                            }
                            if (Boolean.Parse(uCleric.UseLightofRenewal.Checked.ToString()))
                            {
                                if (CoolDowns[0] >= 10)
                                {
                                    CoolDowns[0] = 0;
                                    //progLog(" Light of Renewal");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1400);
                                }
                            }
                            else
                            {
                                if (KeymapGrid.Rows[19].Cells[1].Value != null && KeymapGrid.Rows[19].Cells[1].Value.ToString() != string.Empty)
                                {
                                    if (CoolDowns[0] >= 31)
                                    {
                                        CoolDowns[0] = 0;
                                        //progLog(" Light of Rejuvenation");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    progLog(" Error: No key set for Light of Rejuvenation");
                            }
                        }
                    }
                }
                this.BeginInvoke(new MethodInvoker(delegate { StartBot_btn.Text = "Stopped"; progLog(" Healbot stopped"); }));
                e.Cancel = true;
                return;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            double sbatVAL = double.Parse(StopBottingAfterTime.Text);
            MessageBox.Show(DateTime.Now.AddMinutes(Double.Parse(StopBottingAfterTime.Text)).ToLongTimeString());
        }
        public void PostMouseMove(IntPtr hControl, int x, int y)
        {
            uint WM_MOUSEMOVE = 0x0200;
            
            //PostMessage(this.a, WM_MOUSEMOVE, 0, new IntPtr(y * 0x10000 + x));
            //PostMessage(this.AionPanel.Handle, WM_MOUSEMOVE, 0, new IntPtr(y * 0x10000 + x));
            
        }
        private void ReleaseAION_Click(object sender, EventArgs e)
        {
            SetParent(child, IntPtr.Zero);
            this.Size = new System.Drawing.Size(407, 620);
            EmbedAION.Enabled = true;
            ReleaseAION.Enabled = false;
        }
        private void LaunchAION_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(Application.StartupPath + "/aion.bat"))
            {
                 System.Diagnostics.Process.Start(Application.StartupPath + "/aion.bat");
            }
            else
            {
                MessageBox.Show("Aion.bat not found");
            }
        }
        private void PrintReport_Click(object sender, EventArgs e)
        {
            progLog("   __________________ ");
            progLog("   | Generating report     ");
            progLog("   |                       ");
            progLog("   | Kills: " + Kills.ToString());
            progLog("   | Deaths: " + Deaths.ToString());
            progLog("   | " + ExpEarned_Label.Text);
            progLog("   | Times stuck: " + Stuck.ToString());
            progLog("   | Times rested: " + tRested.ToString());
        }

        // Limits Form
        private void Rest_hpBox_TextChanged(object sender, EventArgs e)
        {
            if (Rest_Health.Text == string.Empty)
                Rest_Health.Undo();
            
            foreach (char c in Rest_Health.Text)
            {
                if (!char.IsDigit(c))
                {
                    Rest_Health.Clear();
                    return;
                }
                if (Rest_Health.Text == "0") 
                    Rest_Health.Clear();
            }
        }
        private void Rest_mpBox_TextChanged(object sender, EventArgs e)
        {
            if (Rest_Mana.Text == string.Empty)
                Rest_Mana.Undo();

            foreach (char c in Rest_Mana.Text)
            {
                if (!char.IsDigit(c))
                {
                    Rest_Mana.Clear();
                    return;
                }
                if (Rest_Mana.Text == "0") 
                    Rest_Mana.Clear();
            }
        }
        private void Rest_UseBandages_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Rest_UseHerbTreatmentValue.Text))
            {
                Rest_UseHerbTreatment.Checked = false;
            }
        }
        private void Rest_UseBandage_pBox_TextChanged(object sender, EventArgs e)
        {
            if (Rest_UseHerbTreatment.Checked && Rest_UseHerbTreatmentValue.Text == string.Empty)
            {
                Rest_UseHerbTreatment.Checked = false;
                return;
            }
            foreach (char c in Rest_UseHerbTreatmentValue.Text)
            {
                if (!char.IsDigit(c))
                    //Rest_UseHerbTreatmentValue.Clear();
                    Rest_UseHerbTreatmentValue.Undo();
                
                if (Rest_UseHerbTreatmentValue.Text == "0")
                    //Rest_UseHerbTreatmentValue.Clear();
                    Rest_UseHerbTreatmentValue.Undo();
            }
            Rest_UseHerbTreatment.Checked = true;
        }
        private void Rest_UseHerbTreatmentHP_CheckedChanged(object sender, EventArgs e)
        {
            if (Rest_UseHerbTreatmentMP.Enabled)
                Rest_UseHerbTreatmentMP.Enabled = false;
            else
                Rest_UseHerbTreatmentMP.Enabled = true;
        }
        private void Rest_UseHerbTreatmentMP_CheckedChanged(object sender, EventArgs e)
        {
            if (Rest_UseHerbTreatmentHP.Enabled)
                Rest_UseHerbTreatmentHP.Enabled = false;
            else
                Rest_UseHerbTreatmentHP.Enabled = true;
        }
        private void Rest_UsePotion_pBox_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in Rest_UsePotionValue.Text)
            {
                if (!char.IsDigit(c))
                    //Rest_UsePotionValue.Clear();
                    Rest_UsePotionValue.Undo();
                
                if (Rest_UsePotionValue.Text == "0")
                    //Rest_UsePotionValue.Clear();
                    Rest_UsePotionValue.Undo();
            }
            if (Rest_UsePotionValue.Text == string.Empty)
                Rest_UsePotionValue.Undo();
        }
        private void Rest_UseHealthPotion_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Rest_UsePotionValue.Text))
            {
                Rest_UseHealthPotion.Checked = false;
            }
            if (Rest_UseManaPotion.Enabled)
                Rest_UseManaPotion.Enabled = false;
            else
                Rest_UseManaPotion.Enabled = true;
        }
        private void Rest_UseManaPotion_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Rest_UsePotionValue.Text))
            {
                Rest_UseManaPotion.Checked = false;
            }
            if (Rest_UseHealthPotion.Enabled)
                Rest_UseHealthPotion.Enabled = false;
            else
                Rest_UseHealthPotion.Enabled = true;
        }
        private void StayAboveHealth_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in StayAboveHealth.Text)
            {
                if (!char.IsDigit(c))
                {
                    StayAboveHealth.Clear();
                    return;
                }
                if (StayAboveHealth.Text == "0") 
                    StayAboveHealth.Clear();
            }
            StayAbove = short.Parse(StayAboveHealth.Text);
        }
        private void FirstBuff_cBox_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstBuff_Duration.Text))
            {
                FirstBuff_cBox.Checked = false; 
            }
            if (KeymapGrid.Rows[5].Cells[1].Value != null && KeymapGrid.Rows[5].Cells[1].Value.ToString() != string.Empty)
            {
                if (FirstBuff_cBox.Checked)
                {
                    //FirstBuff_cBox.Checked = true;
                    BuffTime[0] = 0;
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[5].Cells[1].Value.ToString()), 0);
                }
                else
                    FirstBuff_cBox.Checked = false;
            }
            else
                FirstBuff_cBox.Checked = false;
        }
        private void SecondBuff_cbox_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SecondBuff_Duration.Text))
            {
                SecondBuff_cbox.Checked = false;
            }
            if (KeymapGrid.Rows[6].Cells[1].Value != null && KeymapGrid.Rows[6].Cells[1].Value.ToString() != string.Empty)
            {
                if (SecondBuff_cbox.Checked)
                {
                    //SecondBuff_cbox.Checked = true;
                    BuffTime[1] = 0;
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[6].Cells[1].Value.ToString()), 0);
                }
                else
                    SecondBuff_cbox.Checked = false;
            }
            else
                SecondBuff_cbox.Checked = false;
        }
        private void ThirdBuff_cbox_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ThirdBuff_Duration.Text))
            {
                ThirdBuff_cbox.Checked = false;
            }
            if (KeymapGrid.Rows[7].Cells[1].Value != null && KeymapGrid.Rows[7].Cells[1].Value.ToString() != string.Empty)
            {
                if (ThirdBuff_cbox.Checked)
                {
                    //ThirdBuff_cbox.Checked = true;
                    BuffTime[2] = 0;
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[7].Cells[1].Value.ToString()), 0);
                }
                else
                    ThirdBuff_cbox.Checked = false;
            }
            else
                ThirdBuff_cbox.Checked = false;
        }
        private void FirstBuff_duration_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in FirstBuff_Duration.Text)
            {
                if (!char.IsDigit(c))
                {
                    FirstBuff_Duration.Clear();
                    return;
                }
                if (FirstBuff_Duration.Text == "0") 
                    FirstBuff_Duration.Clear();
            }
        }
        private void SecondBuff_duration_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in SecondBuff_Duration.Text)
            {
                if (!char.IsDigit(c))
                {
                    SecondBuff_Duration.Clear();
                    return;
                }
                if (SecondBuff_Duration.Text == "0") 
                    SecondBuff_Duration.Clear();
            }
        }
        private void ThirdBuff_Duration_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in ThirdBuff_Duration.Text)
            {
                if (!char.IsDigit(c))
                {
                    ThirdBuff_Duration.Clear();
                    return;
                }
                if (ThirdBuff_Duration.Text == "0") 
                    ThirdBuff_Duration.Clear();
            }
        }

       // General Form
        private void ClassLabel_Click(object sender, EventArgs e)
        {
            switch (CurrentClass)
            {
                // .Activate brings the window to front. (Encase it's hidden)
                case "Starter":
                    break;
                case "Sorcerer":
                    uSorcerer.Show();
                    uSorcerer.Activate();
                    if (this.Location.X < 275)
                        uSorcerer.SetDesktopLocation(this.Location.X + 420, this.Location.Y);
                    else
                        uSorcerer.SetDesktopLocation(this.Location.X - 265, this.Location.Y); 
                    break;
                case "Ranger":
                    uRanger.Show();
                    uRanger.Activate();
                    if (this.Location.X < 275)
                        uRanger.SetDesktopLocation(this.Location.X + 420, this.Location.Y);
                    else
                        uRanger.SetDesktopLocation(this.Location.X - 265, this.Location.Y); 
                    break;
                case "Cleric":
                    uCleric.Show();
                    uCleric.Activate();
                    if (this.Location.X < 275)
                        uCleric.SetDesktopLocation(this.Location.X + 420, this.Location.Y);
                    else
                        uCleric.SetDesktopLocation(this.Location.X - 265, this.Location.Y); 
                    break;
                case "Assassin":
                    uAssassin.Show(); 
                    uAssassin.Activate();
                    if (this.Location.X < 275) 
                        uAssassin.SetDesktopLocation(this.Location.X + 420, this.Location.Y);                  
                    else 
                        uAssassin.SetDesktopLocation(this.Location.X-265, this.Location.Y); 
                    break;
                case "Gladiator":
                    uGladiator.Show(); 
                    uGladiator.Activate();
                    if (this.Location.X < 275) 
                        uGladiator.SetDesktopLocation(this.Location.X + 420, this.Location.Y);
                    else 
                        uGladiator.SetDesktopLocation(this.Location.X - 265, this.Location.Y); 
                    break;
                case "Spiritmaster":
                    uSpiritmaster.Show(); 
                    uSpiritmaster.Activate();
                    if (this.Location.X < 275) 
                        uSpiritmaster.SetDesktopLocation(this.Location.X + 420, this.Location.Y);
                    else 
                        uSpiritmaster.SetDesktopLocation(this.Location.X - 265, this.Location.Y); 
                    break;
                case "Chanter":
                    uChanter.Show(); 
                    uChanter.Activate();
                    if (this.Location.X < 275) 
                        uChanter.SetDesktopLocation(this.Location.X + 420, this.Location.Y);                  
                    else 
                        uChanter.SetDesktopLocation(this.Location.X-265, this.Location.Y); 
                    break;
            }
        }
        private void StopBottingAfter_vBox_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in StopBottingAfterTime.Text)
            {
                if (!char.IsDigit(c))
                {
                    StopBottingAfterTime.Clear();
                    return;
                }
                if (StopBottingAfterTime.Text == "0") 
                    StopBottingAfterTime.Clear();
            }
        }
        private void StopBottingAfterDeaths_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in StopBottingAfterDeaths.Text)
            {
                if (!char.IsDigit(c))
                {
                    StopBottingAfterDeaths.Clear();
                    return;
                }
                if (StopBottingAfterDeaths.Text == "0")
                    StopBottingAfterDeaths.Clear();
            }
        }
        private void StopBottingAfter_cBox_CheckedChanged(object sender, EventArgs e)
        {
            StopBottingAfterTime.Enabled = StopBottingAfter.Checked;
            StopAfterDeaths.Enabled = StopBottingAfter.Checked;
            StopAfterMinutes.Enabled = StopBottingAfter.Checked;
            Logout.Enabled = StopBottingAfter.Checked;
            UseReturn.Enabled = StopBottingAfter.Checked;
        }
        private void CraftBox__CheckedChanged(object sender, EventArgs e)
        {
            CraftBotType.Enabled = CraftBox_.Checked;
        }

        // Profile Form
        private void MaxTargetDistance_vbox_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in MaxTargetDistance.Text)
            {
                if (!char.IsDigit(c))
                {
                    MaxTargetDistance.Clear();
                    return;
                }
            }
        }
        private void BlacList_btn_Click(object sender, EventArgs e)
        {
            if (oMemory.OpenProcess(PID))
            {
                if (PlayerHasTarget())
                {
                    for (int i = 0; i < BlackListName.Length; i++)
                    {
                        if (BlackListName[i] == oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Name, 40, true, 0))
                        {
                            return;
                        }
                    }
                    BlackListName[BlackListCount] = oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Name, 40, true, 0);
                    BlackListDisplay.Items.Add(BlackListName[BlackListCount]);
                    BlackListCount++;
                }
            }
        }
        private void MaxTargetLevel_vBox_TextChanged(object sender, EventArgs e)
        {
            foreach (char c in MaxTargetLevel.Text)
            {
                if (!char.IsDigit(c))
                {
                    MaxTargetLevel.Clear();
                    return;
                }
            }
        } 
        private void WaypointAdd_btn_Click(object sender, EventArgs e)
        {
            if (oMemory.OpenProcess(PID))
            {
                if (DeathWaypoints.Checked)
                {
                    if (dWaypointCount == 0)
                        WaypointDisplay.Items.Add("Death Waypoints");
                    
                    dWaypointsX[dWaypointCount] = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X);
                    dWaypointsY[dWaypointCount] = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y);
                    WaypointDisplay.Items.Add("X: " + dWaypointsX[dWaypointCount] + "   Y: " + dWaypointsY[dWaypointCount]);
                    dWaypointCount++;
                }
                else
                {
                    if (WaypointCount == 0)
                        WaypointDisplay.Items.Add("Waypoints");
                    
                    WaypointsX[WaypointCount] = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X);
                    WaypointsY[WaypointCount] = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y);
                    WaypointDisplay.Items.Add("X: " + WaypointsX[WaypointCount] + "   Y: " + WaypointsY[WaypointCount]);
                    WaypointCount++;
                }
            }
        }
        private void AutoWayPoints_btn_Click(object sender, EventArgs e)
        {
            WaypointAdd.PerformClick();

            if (!Timer_AddWaypoints.Enabled)
            {
                Timer_AddWaypoints.Start();
                AutoWayPoints.Text = "Adding waypoints";
            }
            else
            {
                AutoWayPoints.Text = "Auto waypoints";
                Timer_AddWaypoints.Stop();
            }
        }
        private void Timer_AddWaypoints_Tick(object sender, EventArgs e)
        {
            if (oMemory.OpenProcess(PID))
            {
                if (DeathWaypoints.Checked)
                {
                    dWaypointsX[dWaypointCount] = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X);
                    dWaypointsY[dWaypointCount] = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y);
                    WaypointDisplay.Items.Add("X: " + dWaypointsX[dWaypointCount] + "   Y: " + dWaypointsY[dWaypointCount]);
                    WaypointDisplay.SelectedIndex = WaypointDisplay.Items.Count - 1;
                    Console.Beep(900, 400);
                    dWaypointCount++;
                }
                else
                {
                    WaypointsX[WaypointCount] = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X);
                    WaypointsY[WaypointCount] = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y);
                    WaypointDisplay.Items.Add("X: " + WaypointsX[WaypointCount] + "   Y: " + WaypointsY[WaypointCount]);
                    WaypointDisplay.SelectedIndex = WaypointDisplay.Items.Count - 1;
                    Console.Beep(900, 400);
                    WaypointCount++;
                }
            }
        }
        private void NewWaypoints_btn_Click(object sender, EventArgs e)
        {
            WaypointCount = 0; 
            MoveCount = 0; 
            dWaypointCount = 0;

            Waypoint_ListLabel.Text = "Waypoints:";
            movecount_Label.Text = "Current:"; 
            WaypointFile_Label.Text = string.Empty;

            Array.Clear(WaypointsX, 0, WaypointsX.Length); 
            Array.Clear(WaypointsY, 0, WaypointsY.Length);
            Array.Clear(dWaypointsX, 0, dWaypointsX.Length); 
            Array.Clear(dWaypointsY, 0, dWaypointsY.Length);

            for (int i = 0; i < BlackListName.Length; i++)
            {
                BlackListName[i] = null;
            }
            WaypointDisplay.Items.Clear(); 
            BlackListDisplay.Items.Clear();
            MaxTargetLevel.Clear(); 
            MaxTargetDistance.Clear();
        }
        private void SaveWaypoints_btn_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "wpf";
            saveDialog.AddExtension = true;
            saveDialog.RestoreDirectory = true;
            saveDialog.Filter = "Waypoint file (*.wpf)|*.wpf";
            if (WaypointsX[0] == 0)
            {
                MessageBox.Show("No waypoints found");
                return;
            }
            if (MaxTargetDistance.Text.Length == 0 || MaxTargetLevel.Text.Length == 0)
            {
                MessageBox.Show("Enter target distance and level");
                return;
            }
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                System.IO.TextWriter WriteText = new System.IO.StreamWriter(saveDialog.FileName);
                System.IO.FileInfo File = new System.IO.FileInfo(saveDialog.FileName);
                WriteText.WriteLine("Waypoints");
                for (int i = 0; i < WaypointCount; i++)
                {
                    WriteText.WriteLine(WaypointsX[i].ToString() + " " + WaypointsY[i].ToString());
                }
                WriteText.WriteLine("Range and Level");
                WriteText.WriteLine(MaxTargetDistance.Text + " " + MaxTargetLevel.Text);
                WriteText.WriteLine("Death Waypoints");
                for (int i = 0; i < dWaypointCount; i++)
                {
                    WriteText.WriteLine(dWaypointsX[i].ToString() + " " + dWaypointsY[i].ToString());
                }
                WriteText.WriteLine("Soul healer");
                if (SoulHealerNPC.Text != "")
                    WriteText.WriteLine(SoulHealerNPC.Text);
                WriteText.Close();
                WaypointFile_Label.Text = File.Name;
            }
        }
        private void LoadWaypoints_btn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.RestoreDirectory = true;
            openDialog.Filter = "Waypoint file (*.wpf)|*.wpf";
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                bool DeathWP = false;
                bool sHealer = false;
                NewWaypoints.PerformClick();
                foreach (string Line in System.IO.File.ReadAllLines(openDialog.FileName))
                {
                    switch (Line)
                    {
                        case "Waypoints":
                            WaypointDisplay.Items.Add("Waypoints");
                            continue;

                        case "Range and Level": 
                            continue;

                        case "Death Waypoints":
                            WaypointDisplay.Items.Add("Death Waypoints"); 
                            continue;

                        case "Soul healer":
                            sHealer = true;
                            continue;
                    }
                    if (sHealer)
                    {
                        if (Line != "")
                        {
                            SoulHealer.Checked = true;
                            SoulHealerNPC.Text = Line;
                            continue;
                        }
                        else
                            continue;
                    }
                    string[] result = Line.Split(' ');
                    if (result[0].Length <= 2)
                    {
                        MaxTargetDistance.Text = result[0]; 
                        MaxTargetLevel.Text = result[1];
                        DeathWP = true;
                        continue; 
                    }
                    if (!DeathWP)
                    {
                        WaypointsX[WaypointCount] = float.Parse(result[0]);
                        WaypointsY[WaypointCount] = float.Parse(result[1]);
                        WaypointDisplay.Items.Add("X: " + WaypointsX[WaypointCount] + "   Y: " + WaypointsY[WaypointCount]);
                        WaypointCount++;
                    }
                    else
                    {
                        dWaypointsX[dWaypointCount] = float.Parse(result[0]);
                        dWaypointsY[dWaypointCount] = float.Parse(result[1]);
                        WaypointDisplay.Items.Add("X: " + dWaypointsX[dWaypointCount] + "   Y: " + dWaypointsY[dWaypointCount]);
                        dWaypointCount++;
                    }
                }
                Waypoint_ListLabel.Text = "Waypoints: " + WaypointCount.ToString();
                WaypointFile_Label.Text = openDialog.SafeFileName;
            }
        }
        private void SoulHealer_CheckedChanged(object sender, EventArgs e)
        {
            SoulHealerNPC.Enabled = SoulHealer.Checked;
        }

        // Keymap Form
        private void KeymapGrid_KeyDown(object sender, KeyEventArgs e)
        {
            foreach (DataGridViewCell Cell in KeymapGrid.SelectedCells)
            {
                if (Cell.Value != null && Cell.Value.ToString() != string.Empty)
                {
                    if (Cell.Value.ToString() == e.KeyCode.ToString())
                    {
                        //MessageBox.Show("Error! Same key entered!");
                        return;
                    }
                    if (Cell.Value.ToString().Length > 3)
                    {
                        //MessageBox.Show("Error! Cannot alter action values!");
                        return;
                    }
                }
            }
            for (int i = 0; i < KeymapGrid.Rows.Count; i++)
            {
                if (KeymapGrid[1, i].Value != null && KeymapGrid[1, i].Value.ToString() != string.Empty)
                {
                    if (e.KeyCode.ToString() == KeymapGrid[1, i].Value.ToString())
                    {
                        KeymapGrid.CurrentCell.Value = e.KeyCode;
                        KeymapGrid[1, i].Value = null;
                        WaitingForInput.Text = "Unbound: " + KeymapGrid[0, i].Value.ToString();
                        return;
                    }
                }
            }
            switch (e.KeyCode.ToString())
            {
                case "Back":
                    KeymapGrid.CurrentCell.Value = ""; 
                    WaitingForInput.Text = "Key removed...";
                    return;
                case "Delete":
                    KeymapGrid.CurrentCell.Value = ""; 
                    WaitingForInput.Text = "Key removed..."; 
                    return;
                case "Tab":
                    goto Default;
                case "Return":
                    goto Default;
                case "ShiftKey":
                    goto Default;
                case "ControlKey":
                    goto Default;
                case "Menu":
                    goto Default;
                case "Apps":
                    goto Default;
                case "Left":
                    goto Default;
                case "Right":
                    goto Default;
                case "Down":
                    goto Default;
                case "Up":
                    goto Default;
                case "Oem8":
                    goto Default;
                case "Capital":
                    goto Default;
                case "Space":
                    goto Default;
                case "Insert":
                    goto Default;
                case "OemMinus":
                    KeymapGrid.CurrentCell.Value = "-"; 
                    WaitingForInput.Text = "Key accepted!"; 
                    return;
                case "Oemplus":
                    KeymapGrid.CurrentCell.Value = "="; 
                    WaitingForInput.Text = "Key accepted!"; 
                    return;
            }
            WaitingForInput.Text = "Key accepted!";
            KeymapGrid.CurrentCell.Value = e.KeyCode;
            return;
    Default:
            WaitingForInput.Text = "Key invalid!";
        }

        // Combat sequence function
        public string EnterCombat(bool GladiatorTaunt, bool pAmbushed)
        {
            if (oMemory.OpenProcess(PID))
            {
                short tUnreachableCount = 0;
                while (!PlayerDead())
                {
                    if (CurrentClass == "Starter")
                    {
                        do
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(500);
                            tUnreachableCount++;
                            if (TargetIsTargetingPlayer() || TargetIsCasting())
                                break;
                            if (tUnreachableCount >= 20)
                                return "TargetUnreachable";
                        }
                        while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                        while (!PlayerDead() && !TargetIsDead())
                        {
                            if (KeymapGrid.Rows[5].Cells[1].Value != null && KeymapGrid.Rows[5].Cells[1].Value.ToString() != string.Empty)
                            {
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[5].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(4000);
                            }
                        }
                    }
    // SORCERER
                    else if (CurrentClass == "Sorcerer")
                    {
                        if (pAmbushed)
                        {
                            if (Boolean.Parse(uSorcerer.UseWinterBinding.Checked.ToString()))
                            {
                                if (KeymapGrid.Rows[18].Cells[1].Value != null && KeymapGrid.Rows[18].Cells[1].Value.ToString() != string.Empty)
                                {
                                    if (TargetDistanceFromPlayer() <= 12.5)
                                    {
                                        if (CoolDowns[6] >= 61)
                                        {
                                            CoolDowns[6] = 0;
                                            progLog(" Winter Binding - Ambushed");
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[18].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(1400);
                                        }
                                    }
                                }
                            }
                            else if (CoolDowns[1] >= 61)
                            {
                                if (TargetDistanceFromPlayer() <= 15)
                                {
                                    CoolDowns[1] = 0;
                                    progLog(" Rooting - Ambushed");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[11].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1400);
                                }
                            }
                            if (KeymapGrid.Rows[16].Cells[1].Value != null && KeymapGrid.Rows[16].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (TargetDistanceFromPlayer() <= 15)
                                {
                                    if (CoolDowns[5] >= 31)
                                    {
                                        CoolDowns[5] = 0;
                                        progLog(" Blind Leap - Ambushed");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                    else
                                    {
                                        for (int i = 0; i < 25; i++)
                                        {
                                            oMemory.Write(_Address_Base + nmLocation._Address_AutoRunFlag, 8);
                                            Thread.Sleep(20);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            switch (uSorcerer.OpeningSkill)
                            {
                                case "Ice Chain":
                                    progLog(" Ice Chain");
                                    do
                                    {
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            //progLog(" Casting detected");
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                            {
                                                Thread.Sleep(100);
                                            }
                                        }
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                        {
                                            //progLog(" Chain Skill detected"); 
                                            do
                                            {
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(500);
                                                if (TargetIsDead())
                                                    break;
                                            }
                                            while (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0);
                                        }
                                        Thread.Sleep(900);
                                        tUnreachableCount++;
                                        if (TargetIsTargetingPlayer() || TargetIsCasting())
                                            break;
                                        if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 14)
                                            return "TargetUnreachable";
                                    }
                                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                    break;

                                case "Delayed Blast":
                                    if (CoolDowns[7] <= 28)
                                        goto case "Ice Chain";

                                    short Fail = 0;
                                    progLog(" Delayed Blast");
                                    do
                                    {
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            //progLog(" Casting detected");
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                            {
                                                Thread.Sleep(100);
                                            }
                                            Thread.Sleep(1000);
                                            CoolDowns[7] = 0;
                                            goto case "Ice Chain";
                                        }
                                        if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 14)
                                            return "TargetUnreachable";
                                        Fail++;
                                    }
                                    while (Fail != 10);
                                    progLog(" Failed to pull with Delayed Blast"); 
                                    return "TargetUnreachable";
                            }
                        }
                        while (!PlayerDead() && !TargetIsDead())
                        {
                            if (KeymapGrid.Rows[22].Cells[1].Value != null && KeymapGrid.Rows[22].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[0] >= 11)
                                    {
                                        CoolDowns[0] = 0;
                                        //progLog(" Flame Fusion"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[22].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            //progLog(" Casting detected");
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                            {
                                                Thread.Sleep(100);
                                            }
                                        }
                                        Thread.Sleep(900);
                                    }
                                }
                                else
                                    break;
                            }
                            if (Boolean.Parse(uSorcerer.UseFlameCage.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[2] >= 15)
                                    {
                                        CoolDowns[2] = 0;
                                        //progLog(" Flame Cage");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[17].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    break;
                            }
                            else
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[2] >= 15)
                                    {
                                        CoolDowns[2] = 0;
                                        //progLog(" Erosion");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[10].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    break;
                            }
                            if (Boolean.Parse(uSorcerer.UseRoot.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) > oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax) * 30 / 100)
                                    {
                                        if (CoolDowns[1] >= 61)
                                        {
                                            CoolDowns[1] = 0;
                                            //progLog(" Rooting");
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[11].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(1400);
                                            if (Boolean.Parse(uSorcerer.UseBlindLeap.Checked.ToString()))
                                            {
                                                if (KeymapGrid.Rows[16].Cells[1].Value != null && KeymapGrid.Rows[16].Cells[1].Value.ToString() != string.Empty)
                                                {
                                                    if (TargetDistanceFromPlayer() <= 12)
                                                    {
                                                        if (CoolDowns[5] >= 31)
                                                        {
                                                            CoolDowns[5] = 0;
                                                            //progLog(" Blind Leap");
                                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                                            Thread.Sleep(1400);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                    break;
                            }
                            if (uSorcerer.OpeningSkill != "Delayed Blast")
                            {
                                if (!TargetIsDead())
                                {
                                    if (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) > oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax) * 40 / 100)
                                    {
                                        if (CoolDowns[7] >= 31)
                                        {
                                            CoolDowns[7] = 0;
                                            //progLog(" Delayed Blast"); 
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(1400);
                                        }
                                    }
                                }
                                else
                                    break;
                            }
                            if (KeymapGrid.Rows[15].Cells[1].Value != null && KeymapGrid.Rows[15].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[4] >= 11)
                                    {
                                        CoolDowns[4] = 0;
                                        //progLog(" Flame Harpoon");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[15].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                            {
                                                Thread.Sleep(100);
                                            }
                                        }
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    break;
                            }
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[3] >= 2)
                                {
                                    CoolDowns[3] = 0;
                                    //progLog(" Flame Bolt");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[9].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                    {
                                        //progLog(" Casting detected"); 
                                        while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            Thread.Sleep(100);
                                        }
                                    }
                                    Thread.Sleep(500);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                    {
                                        //progLog(" Chain Skill detected"); 
                                        //Thread.Sleep(900);
                                        do
                                        {
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[9].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(500);
                                            if (TargetIsDead())
                                                break;
                                        }
                                        while (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0);
                                    }
                                    Thread.Sleep(900);
                                }
                            }
                            else
                                break;
                            if (BuffTime[6] >= 120)
                            {
                                if (KeymapGrid.Rows[13].Cells[1].Value != null && KeymapGrid.Rows[13].Cells[1].Value.ToString() != string.Empty)
                                {
                                    BuffTime[6] = 0;
                                    progLog(" Buff Stone Skin...");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1400);
                                }
                                else
                                    progLog(" Error: No key set for Stone Skin...");
                            }
                            if (KeymapGrid.Rows[20].Cells[1].Value != null && KeymapGrid.Rows[20].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (TargetDistanceFromPlayer() <= 5)
                                    {
                                        if (CoolDowns[8] >= 3)
                                        {
                                            CoolDowns[8] = 0;
                                            //progLog(" Freezing Wind");
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[20].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(1400);
                                        }
                                    }
                                }
                                else
                                    break;
                            }
                            if (Boolean.Parse(uSorcerer.UseWinterBinding.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) > oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax) * 30 / 100)
                                    {
                                        if (TargetDistanceFromPlayer() <= 12.5)
                                        {
                                            if (CoolDowns[6] >= 61)
                                            {
                                                CoolDowns[6] = 0;
                                                //progLog(" Winter Binding"); 
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[18].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(1400);
                                                if (Boolean.Parse(uSorcerer.UseBlindLeap.Checked.ToString()))
                                                {
                                                    //if (KeymapGrid.Rows[16].Cells[1].Value != null && KeymapGrid.Rows[16].Cells[1].Value.ToString() != string.Empty)
                                                    if (TargetDistanceFromPlayer() <= 12)
                                                    {
                                                        if (CoolDowns[5] >= 31)
                                                        {
                                                            CoolDowns[5] = 0;
                                                            //progLog(" Blind Leap"); 
                                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                                            Thread.Sleep(1500);
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                    break;
                            }
                            if (Boolean.Parse(uSorcerer.UseBlindLeap.Checked.ToString()))
                            {
                                if (KeymapGrid.Rows[16].Cells[1].Value != null && KeymapGrid.Rows[16].Cells[1].Value.ToString() != string.Empty)
                                {
                                    if (!TargetIsDead())
                                    {
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) > oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax) * 30 / 100)
                                        {
                                            if (TargetDistanceFromPlayer() <= 15)
                                            {
                                                if (CoolDowns[5] >= 31)
                                                {
                                                    CoolDowns[5] = 0;
                                                    //progLog(" Blind Leap");
                                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                                    Thread.Sleep(1400);
                                                }
                                            }
                                        }
                                    }
                                    else
                                        break;
                                }
                                else
                                    progLog(" Error. No key set for Winter Binding");
                            }
                        }
                    }
    // RANGER
                    else if (CurrentClass == "Ranger")
                    {
                        bool EntanglingShot = false;
                        if (pAmbushed)
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(500);
                        }
                        else
                        {
                            switch (uRanger.OpeningSKill)
                            {
                                case "Attack":
                                    do
                                    {
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        tUnreachableCount++;
                                        if (TargetIsTargetingPlayer() || TargetIsCasting())
                                            break;
                                        if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 14)
                                            return "TargetUnreachable";
                                    }
                                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                    break;
                                case "Entangling Shot":
                                    EntanglingShot = true;
                                    CoolDowns[0] = 0;
                                    //progLog(" Entangling Shot");
                                    do
                                    {
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        tUnreachableCount++;
                                        if (TargetIsTargetingPlayer() || TargetIsCasting())
                                            break;
                                        if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 14)
                                            return "TargetUnreachable";
                                    }
                                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                    break;
                            }
                        }
                        while (!PlayerDead() && !TargetIsDead())
                        {
                            if (!TargetIsDead())
                            {
                                if (KeymapGrid.Rows[15].Cells[1].Value != null && KeymapGrid.Rows[15].Cells[1].Value.ToString() != string.Empty)
                                {
                                    if (CoolDowns[5] >= 13)
                                    {
                                        CoolDowns[5] = 0;
                                        //progLog(" Stunning Shot");
                                    StunChain:
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[15].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                        {
                                            //progLog(" Chain Skill detected");
                                            Thread.Sleep(1000);
                                            goto StunChain;
                                        }
                                        else
                                            Thread.Sleep(1000);
                                    }
                                }
                            }
                            else
                                break;
                            if (Boolean.Parse(uRanger.UseDevotion.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[3] >= 31)
                                    {
                                        CoolDowns[3] = 0;
                                        //progLog(" Devotion");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[10].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1500);
                                    }
                                }
                                else
                                    break;
                            }
                            if (!EntanglingShot)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[0] >= 17)
                                    {
                                        CoolDowns[0] = 0;
                                        //progLog(" Entangling Shot");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1500);
                                    }
                                }
                                else
                                    break;
                            }
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[1] >= 9)
                                {
                                    CoolDowns[1] = 0;
                                    //progLog(" Swift Shot"); 
                                SwiftChain:
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[9].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                    {
                                        //progLog(" Chain Skill detected");
                                        Thread.Sleep(1000);
                                        goto SwiftChain;
                                    }
                                    else
                                        Thread.Sleep(1000);
                                }
                            }
                            else
                                break;
                            if (Boolean.Parse(uRanger.UseDeadShot.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[2] >= 3)
                                    {
                                        CoolDowns[2] = 0;
                                        //progLog(" Deadshot");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1500);
                                    }
                                }
                                else break;
                            }
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[4] >= 31)
                                {
                                    CoolDowns[4] = 0;
                                    //progLog(" Focused Evasion");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[11].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1500);
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                }
                            }
                            else break;
                        }
                    }
    // CLERIC
                    else if (CurrentClass == "Cleric")
                    {
                        /* 0 = Light of Renewal / Light of Rejuvenation
                         * 1 = Hallowed Strike
                         * 2 = Smite
                         * 3 = Holy Servant
                         * 4 = Storm of Aion
                         * 5 = Punishing Wind
                         * 6 = Chastise
                         * 7 = Blessed Shield
                         * 8 =
                         * 9 = 
                         * 10 =
                         * 
                         */
                        if (pAmbushed)
                        {
                        }
                        else
                        {
                            switch (uCleric.OpeningSKill)
                            {
                                case "Smite":
                                    short TargetUnreachableVar = 14;
                                    progLog(" Pulling with Smite");
                                    do
                                    {
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            //progLog(" Casting detected");
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                            {
                                                Thread.Sleep(100);
                                            }
                                        }
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                        {
                                            //progLog(" Chain Skill detected"); 
                                            Thread.Sleep(900);
                                            do
                                            {
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(500);
                                                if (TargetIsDead())
                                                    break;
                                            }
                                            while (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0);
                                        }
                                        Thread.Sleep(900);
                                        tUnreachableCount++;
                                        if (TargetIsTargetingPlayer() || TargetIsCasting())
                                            break;
                                        if (TargetMoving())
                                            TargetUnreachableVar = 20;
                                        if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= TargetUnreachableVar)
                                            return "TargetUnreachable";
                                    }
                                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                    break;
                                case "Storm of Aion":
                                    if (CoolDowns[4] <= 29)
                                        goto case "Smite";
                                    
                                    short TargetUnreachableVarA = 14;
                                    progLog(" Pulling with Storm of Aion");
                                    do
                                    {
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            //progLog(" Casting detected");
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                            {
                                                Thread.Sleep(100);
                                            }
                                        }
                                        tUnreachableCount++;
                                        if (TargetIsTargetingPlayer() || TargetIsCasting())
                                            break;
                                        if (TargetMoving())
                                            TargetUnreachableVar = 20;
                                        if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= TargetUnreachableVarA)
                                            return "TargetUnreachable";
                                    }
                                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                    CoolDowns[4] = 0;
                                    Thread.Sleep(500);
                                    if (KeymapGrid.Rows[22].Cells[1].Value != null && KeymapGrid.Rows[22].Cells[1].Value.ToString() != string.Empty)
                                    {
                                        //progLog(" Rooting");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[22].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                    break;
                            }
                        }
                        while (!PlayerDead() && !TargetIsDead())
                        {
                            // CHASTISE
                            if (KeymapGrid.Rows[21].Cells[1].Value != null && KeymapGrid.Rows[21].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[6] >= 17)
                                    {
                                        CoolDowns[6] = 0;
                                        //progLog(" Chastise");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[21].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    break;
                            }
                            // PUNISHING WIND
                            if (KeymapGrid.Rows[25].Cells[1].Value != null && KeymapGrid.Rows[25].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[5] >= 17)
                                    {
                                        CoolDowns[5] = 0;
                                        //progLog(" Punishing Wind"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[25].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                        {
                                            //progLog(" Chain Skill detected"); 
                                            do
                                            {
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[25].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(500);
                                                if (TargetIsDead())
                                                    break;
                                            }
                                            while (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0);
                                        }
                                        Thread.Sleep(900);
                                    }
                                }
                                else
                                    break;
                            }
                            if (PlayerHealthBelowPercent(StayAbove))
                                goto HealLoop;

                            // STORM OF AION
                            if (KeymapGrid.Rows[16].Cells[1].Value != null && KeymapGrid.Rows[16].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[4] >= 31)
                                    {
                                        CoolDowns[4] = 0;
                                        //progLog(" Storm of Aion");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            //progLog(" Casting detected"); 
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                            {
                                                Thread.Sleep(100);
                                            }
                                        }
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    break;
                            }
                            // SMITE
                            if (Boolean.Parse(uCleric.UseSmite.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[2] >= 5)
                                    {
                                        CoolDowns[2] = 0;
                                        //progLog(" Smite"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            //progLog(" Casting detected");
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                            {
                                                Thread.Sleep(100);
                                            }
                                        }
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                        {
                                            //progLog(" Chain Skill detected"); 
                                            do
                                            {
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(500);
                                                if (TargetIsDead())
                                                    break;
                                            }
                                            while (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0);
                                        }
                                        Thread.Sleep(900);
                                    }
                                }
                                else
                                    break;
                            }
                            if (PlayerHealthBelowPercent(StayAbove))
                                goto HealLoop;

                            // HALLOWED STRIKE
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[1] >= 9)
                                {
                                    CoolDowns[1] = 0;
                                    //progLog(" Hallowed Strike");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[11].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                    {
                                        //progLog(" Chain Skill detected"); 
                                        Thread.Sleep(900);
                                        do
                                        {
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[11].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(500);
                                            if (TargetIsDead())
                                                break;
                                        }
                                        while (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0);
                                    }
                                    Thread.Sleep(900);
                                }
                            }
                            else
                                break;
                            // HOLY SERVANT
                            if (Boolean.Parse(uCleric.UseHolyServant.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[3] >= 31)
                                    {
                                        CoolDowns[3] = 0;
                                        //progLog(" Holy Servant");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[15].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    break;
                            }
                        HealLoop:
                            // HEALING LOOP
                            if (!TargetIsDead())
                            {
                                if (PlayerHealthBelowPercent(StayAbove))
                                {

                                    progLog(" Healing");
                                    while (!PlayerDead() && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * StayAbove / 100)
                                    {
                                        if (KeymapGrid.Rows[24].Cells[1].Value != null && KeymapGrid.Rows[24].Cells[1].Value.ToString() != string.Empty)
                                        {
                                            if (CoolDowns[7] >= 121)
                                            {
                                                CoolDowns[7] = 0;
                                                //progLog(" Blessed Shield");
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[24].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(1400);
                                            }
                                        }
                                        if (KeymapGrid.Rows[23].Cells[1].Value != null && KeymapGrid.Rows[23].Cells[1].Value.ToString() != string.Empty)
                                        {
                                            int RadiantPercent;
                                            if (StayAbove - 30 <= 0)
                                                RadiantPercent = 35;
                                            else
                                                RadiantPercent = StayAbove - 35;

                                            if (PlayerHealthBelowPercent(RadiantPercent))
                                            //if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * PercentCalculated / 100)
                                            {
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[23].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(500);
                                                if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                                {
                                                    //progLog(" Casting detected");
                                                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                                    {
                                                        Thread.Sleep(100);
                                                    }
                                                    Thread.Sleep(1400);
                                                }
                                            }
                                        }
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                        {
                                            //progLog(" Casting detected");
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                            {
                                                Thread.Sleep(100);
                                            }
                                            Thread.Sleep(1400);
                                        }
                                        if (Boolean.Parse(uCleric.UseLightofRenewal.Checked.ToString()))
                                        {
                                            if (CoolDowns[0] >= 10)
                                            {
                                                CoolDowns[0] = 0;
                                                //progLog(" Light of Renewal");
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(1400);
                                            }
                                        }
                                        else
                                        {
                                            if (KeymapGrid.Rows[19].Cells[1].Value != null && KeymapGrid.Rows[19].Cells[1].Value.ToString() != string.Empty)
                                            {
                                                if (CoolDowns[0] >= 31)
                                                {
                                                    CoolDowns[0] = 0;
                                                    //progLog(" Light of Renewal");
                                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                                    Thread.Sleep(1400);
                                                }
                                            }
                                            else
                                                progLog(" Error: No key set for Light of Rejuvenation");
                                        }
                                    }
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);
                                    Thread.Sleep(200);
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);
                                    Thread.Sleep(200);
                                }
                            }
                            if (Boolean.Parse(uCleric.UseLightofRenewal.Checked.ToString()))
                            {
                                if (CoolDowns[0] >= 10)
                                {
                                    CoolDowns[0] = 0;
                                    //progLog(" Light of Renewal");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1400);
                                }
                            }
                            else
                            {
                                if (KeymapGrid.Rows[19].Cells[1].Value != null && KeymapGrid.Rows[19].Cells[1].Value.ToString() != string.Empty)
                                {
                                    if (CoolDowns[0] >= 31)
                                    {
                                        CoolDowns[0] = 0;
                                        //progLog(" Light of Renewal");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    progLog(" Error: No key set for Light of Rejuvenation");
                            }
                        }
                    }
    // ASSASSIN
                    else if (CurrentClass == "Assassin")
                    {
                        /* 0 = Swift Edge
                         * 1 = Focused Evasion
                         * 2 = Surprise Attack
                         * 3 = Devotion
                         * 4 = Pain Rune
                         * 5 = Fang Strike
                         * 6 = Rune Carve
                         * 7 = Dash Attack
                         * 8 = Blood Rune
                         * 9 = Ripclaw
                         * 10 = Ambush
                         * 11 = Darkness Rune
                         * 12 = Weakening Blow
                         * 13 = Killers Eye
                         * 14 = Flurry
                         */
                        switch (uAssassin.OpeningSKill)
                        {
                            case "Attack N Dash":
                                // FLURRY
                                if (Boolean.Parse(uAssassin.UseFlurry.Checked.ToString()))
                                {
                                    if (!TargetIsDead())
                                    {
                                        if (CoolDowns[14] >= 121)
                                        {
                                            CoolDowns[14] = 0;
                                            //progLog(" Flurry"); 
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[25].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(1400);
                                        }
                                    }
                                    else
                                        break;
                                }
                                if (TargetDistanceFromPlayer() >= 8)
                                {
                                    if (KeymapGrid.Rows[19].Cells[1].Value != null && KeymapGrid.Rows[19].Cells[1].Value.ToString() != string.Empty)
                                    {
                                        if (CoolDowns[10] >= 41)
                                        {
                                            progLog(" Opening with Ambush");
                                            do
                                            {
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(500);
                                                tUnreachableCount++;
                                                if (TargetIsTargetingPlayer())
                                                    break;
                                                if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 20)
                                                    return "TargetUnreachable";
                                            }
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                            CoolDowns[10] = 0;
                                            Thread.Sleep(1000);
                                        }
                                        else if (CoolDowns[7] >= 41)
                                        {
                                            progLog(" Opening with Dash Attack");
                                            do
                                            {
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(500);
                                                tUnreachableCount++;
                                                if (TargetIsTargetingPlayer())
                                                    break;
                                                if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 20)
                                                    return "TargetUnreachable";
                                            }
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                            CoolDowns[7] = 0;
                                            Thread.Sleep(1000);
                                        }
                                        else
                                        {
                                            progLog(" Opening with Attack");
                                            do
                                            {
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(500);
                                                tUnreachableCount++;
                                                if (TargetIsTargetingPlayer())
                                                    break;
                                                if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 20)
                                                    return "TargetUnreachable";
                                            }
                                            while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                        }
                                        break;
                                    }
                                    else if (CoolDowns[7] >= 41)
                                    {
                                        progLog(" Opening with Dash Attack");
                                        do
                                        {
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(500);
                                            tUnreachableCount++;
                                            if (TargetIsTargetingPlayer())
                                                break;
                                            if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 20)
                                                return "TargetUnreachable";
                                        }
                                        while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                        CoolDowns[7] = 0;
                                        Thread.Sleep(1000);
                                    }
                                    else
                                    {
                                        progLog(" Opening with Attack");
                                        do
                                        {
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(500);
                                            tUnreachableCount++;
                                            if (TargetIsTargetingPlayer())
                                                break;
                                            if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 20)
                                                return "TargetUnreachable";
                                        }
                                        while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                    }
                                    break;
                                }
                                else
                                {
                                    progLog(" Opening with Attack");
                                    do
                                    {
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        tUnreachableCount++;
                                        if (TargetIsTargetingPlayer())
                                            break;
                                        if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 20)
                                            return "TargetUnreachable";
                                    }
                                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                }
                                break;
                        }
                        while (!TargetIsDead() && !PlayerDead())
                        {
                            // DEVOTION
                            if (Boolean.Parse(uAssassin.UseDevotion.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[3] >= 31)
                                    {
                                        CoolDowns[3] = 0;
                                        //progLog(" Devotion"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    break;
                            }

                            // WEAKENING BLOW
                            if (KeymapGrid.Rows[22].Cells[1].Value != null && KeymapGrid.Rows[22].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[12] >= 61)
                                    {
                                        CoolDowns[12] = 0;
                                        //progLog(" Weakening Blow");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[22].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    break;
                            }

                            // KILLERS EYE
                            if (KeymapGrid.Rows[21].Cells[1].Value != null && KeymapGrid.Rows[21].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[9] >= 61)
                                    {
                                        if (KeymapGrid.Rows[24].Cells[1].Value != null && KeymapGrid.Rows[24].Cells[1].Value.ToString() != string.Empty)
                                        {
                                            if (CoolDowns[13] >= 61)
                                            {
                                                CoolDowns[13] = 0;
                                                //progLog(" Buff Killers Eye"); 
                                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[24].Cells[1].Value.ToString()), 0);
                                                Thread.Sleep(1400);
                                            }
                                        }
                                        CoolDowns[9] = 0;
                                        //progLog(" Ripclaw"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[21].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                        goto RuneStrike;
                                    }
                                }
                                else
                                    break;
                            }

                            // SWIFT EDGE
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[0] >= 8)
                                { 
                                    CoolDowns[0] = 0;
                                    //progLog(" Swift Edge"); 
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                    {
                                        //progLog(" Chain Skill detected"); 
                                        Thread.Sleep(900);
                                        do
                                        {
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(500);
                                            if (TargetIsDead())
                                                break;
                                        }
                                        while (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0);
                                    }
                                    Thread.Sleep(900);
                                }
                            }
                            else
                                break;

                            // SURPRISE ATTACK
                            if (Boolean.Parse(uAssassin.UseSurpriseAttack.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[2] >= 17)
                                    {
                                        CoolDowns[2] = 0;
                                        //progLog(" Surprise Attack");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[10].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else
                                    break;
                            }

                            // RUNE CARVE
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[6] >= 8)
                                {
                                    CoolDowns[6] = 0;
                                    //progLog(" Rune Carve"); 
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                    {
                                        //progLog(" Chain Skill detected"); 
                                        Thread.Sleep(900);
                                        do
                                        {
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(500);
                                            if (TargetIsDead())
                                                break;
                                        }
                                        while (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0);
                                    }
                                    Thread.Sleep(900);
                                }
                            }
                            else
                                break;

                            // FANG STRIKE
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[5] >= 7)
                                {
                                    CoolDowns[5] = 0;
                                    //progLog(" Fang Strike");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[11].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                    {
                                        //progLog(" Chain Skill detected"); 
                                        Thread.Sleep(900);
                                        do
                                        {
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[11].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(500);
                                            if (TargetIsDead())
                                                break;
                                        }
                                        while (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0);
                                    }
                                    Thread.Sleep(900);
                                }
                            }
                            else
                                break;

                            // RUNE STRIKE
                        RuneStrike:
                            if (!TargetIsDead())
                            {
                                if (KeymapGrid.Rows[24].Cells[1].Value != null && KeymapGrid.Rows[24].Cells[1].Value.ToString() != string.Empty)
                                {
                                    if (CoolDowns[13] >= 61)
                                    {
                                        CoolDowns[13] = 0;
                                        //progLog(" Buff Killers Eye"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[24].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                if (KeymapGrid.Rows[23].Cells[1].Value != null && KeymapGrid.Rows[23].Cells[1].Value.ToString() != string.Empty)
                                {
                                    if (CoolDowns[11] >= 181)
                                    {
                                        CoolDowns[11] = 0;
                                        //progLog(" Darkness Rune"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[23].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                    else if (KeymapGrid.Rows[18].Cells[1].Value != null && KeymapGrid.Rows[18].Cells[1].Value.ToString() != string.Empty)
                                    {
                                        if (CoolDowns[8] >= 61)
                                        {
                                            CoolDowns[8] = 0;
                                            //progLog(" Blood Rune"); 
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[18].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(1400);
                                        }
                                        else if (CoolDowns[4] >= 19)
                                        {
                                            CoolDowns[4] = 0;
                                            //progLog(" Pain Rune"); 
                                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                                            Thread.Sleep(1400);
                                        }
                                    }
                                    else if (CoolDowns[4] >= 19)
                                    {
                                        CoolDowns[4] = 0;
                                        //progLog(" Pain Rune"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else if (KeymapGrid.Rows[18].Cells[1].Value != null && KeymapGrid.Rows[18].Cells[1].Value.ToString() != string.Empty)
                                {
                                    if (CoolDowns[8] >= 61)
                                    {
                                        CoolDowns[8] = 0;
                                        //progLog(" Blood Rune"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[18].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                    else if (CoolDowns[4] >= 19)
                                    {
                                        CoolDowns[4] = 0;
                                        //progLog(" Pain Rune");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1400);
                                    }
                                }
                                else if (CoolDowns[4] >= 19)
                                {
                                    CoolDowns[4] = 0;
                                    //progLog(" Pain Rune");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1400);
                                }
                            }
                            else
                                break;

                            // FOCUSED EVASION
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[1] >= 31)
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[9].Cells[1].Value.ToString()), 0);
                                    CoolDowns[1] = 0;
                                    //progLog(" Focused Evasion");
                                    Thread.Sleep(1400);
                                }
                            }
                            else
                                break;
                        }
                    }
    // GLADIATOR
                    else if (CurrentClass == "Gladiator")
                    {
                        bool Cleave = false;
                        switch (uGladiator.OpeningSKill)
                        {
                            case "Attack":
                                do
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)System.Windows.Forms.Keys.C, 0);
                                    Thread.Sleep(500);
                                    tUnreachableCount++;
                                    if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 15)
                                        return "TargetUnreachable";
                                }
                                while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                break;
                            case "Cleave":
                                Cleave = true;
                                do
                                {
                                CleaveChain:
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                    {
                                        Thread.Sleep(1000);
                                        goto CleaveChain;
                                    }
                                    else
                                        Thread.Sleep(1000);
                                    tUnreachableCount++;
                                    if (TargetIsTargetingPlayer() || TargetIsCasting())
                                        break;
                                    if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 15)
                                        return "TargetUnreachable";
                                }
                                while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                break;
                            case "Taunt":
                                do
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    tUnreachableCount++;
                                    if (TargetIsTargetingPlayer() || TargetIsCasting())
                                        break;
                                    if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 14)
                                        return "TargetUnreachable";
                                }
                                while (TargetDistanceFromPlayer() > 10);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);
                                Thread.Sleep(100);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);
                                break;
                        }
                        while (!PlayerDead() && !TargetIsDead())
                        {
                            if (!Cleave)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[10] >= 19)
                                    {
                                        CoolDowns[10] = 0;
                                        //progLog(" Cleave");
                                    CleaveChain:
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0 && !TargetIsDead())
                                        {
                                            //progLog(" Chain Skill detected");
                                            Thread.Sleep(1000);
                                            goto CleaveChain;
                                        }
                                        else
                                            Thread.Sleep(1000);
                                    }
                                }
                                else
                                    break;
                            }

                            if (Boolean.Parse(uGladiator.UseWeakeningBlow.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[0] >= 31)
                                    {
                                        CoolDowns[0] = 0;
                                        //progLog(" Weakening Blow");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1500);
                                    }
                                }
                                else
                                    break;
                            }

                            if (Boolean.Parse(uGladiator.UseUnwaveringDevotion.Checked.ToString()))
                            {
                                if (CoolDowns[1] >= 181)
                                {
                                    CoolDowns[1] = 0;
                                    //progLog(" Unwavering Devotion"); 
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[21].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1750);
                                }
                            }

                            if (!TargetIsDead())
                            {
                                if (CoolDowns[2] >= 13)
                                {
                                    if (Boolean.Parse(uGladiator.PreferWSB.Checked.ToString()))
                                    {
                                        CoolDowns[2] = 0;
                                        //progLog(" Severe Blow");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[11].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1500);
                                    }
                                    else
                                    {
                                        CoolDowns[2] = 0;
                                        //progLog(" Body Smash");
                                    BodyChain:
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0 && !TargetIsDead())
                                        {
                                            //progLog(" Chain Skill detected");
                                            Thread.Sleep(1000);
                                            goto BodyChain;
                                        }
                                        else
                                            Thread.Sleep(1000);
                                    }
                                }
                            }
                            else
                                break;

                            if (!TargetIsDead())
                            {
                                if (CoolDowns[4] >= 11)
                                {
                                    CoolDowns[4] = 0;
                                    //progLog(" Ferocious Strike");
                                FerociousChain:
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0 && !TargetIsDead())
                                    {
                                        //progLog(" Chain Skill detected");
                                        Thread.Sleep(1000);
                                        goto FerociousChain;
                                    }
                                    else
                                        Thread.Sleep(1000);
                                }
                            }
                            else
                                break;

                            if (Boolean.Parse(uGladiator.UseSeismicWave.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (TargetDistanceFromPlayer() < 9 && CoolDowns[5] >= 11)
                                    {
                                        CoolDowns[5] = 0;
                                        //progLog(" Seismic Wave");
                                    SeismicChain:
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[15].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(500);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0 && !TargetIsDead())
                                        {
                                            //progLog(" Chain Skill detected");
                                            Thread.Sleep(1000);
                                            goto SeismicChain;
                                        }
                                        else
                                            Thread.Sleep(1000);
                                    }
                                }
                                else
                                    break;
                            }

                            if (!TargetIsDead())
                            {
                                if (Boolean.Parse(uGladiator.UseImprovedStamina.Checked.ToString()) && CoolDowns[6] >= 181 && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * 60 / 100 && !TargetIsDead())
                                {
                                    CoolDowns[6] = 0;
                                    //progLog(" Improved stamina"); 
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[10].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1500);
                                }
                                if (PlayerLevel() >= 34 && CoolDowns[7] >= 601 && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * 50 / 100 && !TargetIsDead())
                                {
                                    CoolDowns[7] = 0;
                                    //progLog(" Stamina Recovery");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[20].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1500);
                                }
                                if (PlayerLevel() >= 37 && CoolDowns[8] >= 601 && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * 40 / 100 && !TargetIsDead())
                                {
                                    CoolDowns[8] = 0;
                                    //progLog(" Wall of Steel"); 
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[22].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1500);
                                }
                            }
                            else
                                break;

                            if (KeymapGrid.Rows[17].Cells[1].Value != null && KeymapGrid.Rows[18].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[3] >= 181)
                                    {
                                        CoolDowns[3] = 0;
                                        //progLog(" Aerial Lockdown"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[18].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1500);
                                    }
                                }
                                else
                                    break;
                            }
                            // else 
                            //this.BeginInvoke(new MethodInvoker(delegate { progLog(" Error. No key set for Aerial Lockdown"); }));

                            if (Boolean.Parse(uGladiator.UseDaevicFury.Checked.ToString()))
                            {
                                if (CoolDowns[9] >= 1801 && oMemory.ReadShort(_Address_Base + nmLocation._Address_LocalPlayer_DP) >= 2000)
                                {
                                    CoolDowns[9] = 0;
                                    //progLog(" Daevic Fury"); 
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[23].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1500);
                                }
                            }

                            if (Boolean.Parse(uGladiator.UsePiercingWave.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[11] >= 61)
                                    {
                                        CoolDowns[11] = 0;
                                        //progLog(" Piercing Wave"); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[17].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1500);
                                    }
                                }
                                else
                                    break;
                            }

                            if (GladiatorTaunt)
                            {
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(1500);
                            }
                        }
                    } /*
                    else if (CurrentClass == "Spiritmaster")
                    {
                        if (!SkipOpeningSkill)
                        {
                            switch (uSpiritmaster.OpeningSKill)
                            {
                                case "Ice Chain":
                                    do
                                    {
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)System.Windows.Forms.Keys.D2, 0);
                                        Thread.Sleep(500);
                                        if (TargetIsTargetingPlayer() || TargetIsCasting()) break;
                                        tUnreachableCount++;
                                        if (tUnreachableCount >= 14) return "TargetUnreachable";
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.F12, 0);
                                        Thread.Sleep(500);
                                    }
                                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                    break;

                                case "Pet Attack":
                                    do
                                    {
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.F12, 0);
                                        Thread.Sleep(500);
                                        if (TargetIsCasting()) break;
                                        tUnreachableCount++;
                                        if (tUnreachableCount >= 20) return "TargetUnreachable";
                                    }
                                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                    break;
                            }
                        }
                        do
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_3, 0);
                            Thread.Sleep(KeyStrokeDelay);
                            if (bool.Parse(uSpiritmaster.SpiritMaster_StoneSkin.Checked.ToString()))
                            {
                                if (double.Parse((DateTime.Now.TimeOfDay - uSpiritmaster.SpiritTime[0]).TotalMinutes.ToString()) >= 2)
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_7, 0);
                                    Thread.Sleep(KeyStrokeDelay);
                                    uSpiritmaster.SpiritTime[0] = DateTime.Now.TimeOfDay;
                                }
                            }
                            if (bool.Parse(uSpiritmaster.SpiritMaster_UseRoot.Checked.ToString()))
                            {
                                if (double.Parse((DateTime.Now.TimeOfDay - uSpiritmaster.SpiritTime[2]).TotalSeconds.ToString()) >= 60)
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_4, 0);
                                    Thread.Sleep(KeyStrokeDelay);
                                    uSpiritmaster.SpiritTime[2] = DateTime.Now.TimeOfDay;
                                }
                            }
                            if (bool.Parse(uSpiritmaster.SpiritMaster_TBC.Checked.ToString()))
                            {
                                if (double.Parse((DateTime.Now.TimeOfDay - uSpiritmaster.SpiritTime[1]).TotalSeconds.ToString()) >= 12)
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_8, 0);
                                    Thread.Sleep(KeyStrokeDelay);
                                    uSpiritmaster.SpiritTime[1] = DateTime.Now.TimeOfDay;
                                }
                            }
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_2, 0);
                            if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_IsCasting) > 0)
                            {
                                Thread.Sleep(2250);
                            }
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_2, 0);
                            Thread.Sleep(KeyStrokeDelay);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_1, 0);
                            if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_IsCasting) > 0)
                            {
                                Thread.Sleep(2250);
                            }
                            Thread.Sleep(KeyStrokeDelay);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_1, 0);
                            Thread.Sleep(KeyStrokeDelay);
                        }
                        while (!TargetIsDead() && !PlayerDead());
                    } */
                    else if (CurrentClass == "Chanter")
                    {
                        switch (uChanter.OpeningSKill)
                        {
                            case "Smite":
                                short TargetUnreachableVar = 14;
                                do
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[26].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(400);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                    {
                                        Thread.Sleep((int)oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime));
                                    }
                                    tUnreachableCount++;
                                    if (TargetIsTargetingPlayer() || TargetIsCasting())
                                        break;
                                    if (TargetMoving())
                                        TargetUnreachableVar = 20;
                                    if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= TargetUnreachableVar)
                                        return "TargetUnreachable";
                                }
                                while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                Thread.Sleep(100);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(100);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                                break;

                            case "Attack":
                                do
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(500);
                                    tUnreachableCount++;
                                    if (TargetIsTargetingPlayer() || TargetIsCasting())
                                        break;
                                    if (ErrorScan() == "ObstacleBlocking" || tUnreachableCount >= 14)
                                        return "TargetUnreachable";
                                }
                                while (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax));
                                break;
                        }
                        while (!TargetIsDead() && !PlayerDead())
                        {
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[2] >= 30)
                                {
                                    //progLog(" Focused parry");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[23].Cells[1].Value.ToString()), 0);
                                    CoolDowns[2] = 0;
                                    Thread.Sleep(1500);
                                }
                            }
                            else break;
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[4] >= 9)
                                {
                                    CoolDowns[4] = 0;
                                    //progLog(" Hallowed Strike");
                                HallowedChain:
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[17].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1000);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                    {
                                        //progLog(" Chain Skill detected"); 
                                        goto HallowedChain;
                                    }
                                    Thread.Sleep(500);
                                }
                            }
                            else break;
                            if (!TargetIsDead())
                            {
                                if (CoolDowns[3] >= 9)
                                {
                                    CoolDowns[3] = 0;
                                    //progLog(" Meteor Strike"); 
                                MeteorChain:
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[18].Cells[1].Value.ToString()), 0);
                                    Thread.Sleep(1000);
                                    if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                    {
                                        //progLog(" Chain Skill detected");
                                        goto MeteorChain;
                                    }
                                    else Thread.Sleep(500);
                                }
                            }
                            else break;
                            if (KeymapGrid.Rows[19].Cells[1].Value != null && KeymapGrid.Rows[19].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[9] >= 13)
                                    {
                                        //progLog(" Soul Strike");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                        CoolDowns[9] = 0;
                                        Thread.Sleep(1500);
                                    }
                                }
                                else break;
                            }
                            if (KeymapGrid.Rows[29].Cells[1].Value != null && KeymapGrid.Rows[29].Cells[1].Value.ToString() != string.Empty)
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[10] >= 61)
                                    {
                                        CoolDowns[10] = 0;
                                        //progLog(" Annihilation"); 
                                    AnnihilateChain:
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[29].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(1000);
                                        if (oMemory.ReadInt(_Address_Base + nmLocation.ChainSkill_ID, nmLocation.offsets_ChainSkill_ID) > 0)
                                        {
                                            //progLog(" Chain Skill detected");
                                            goto AnnihilateChain;
                                        }
                                        else Thread.Sleep(500);
                                    }
                                }
                                else break;
                            }
                            if (CoolDowns[0] >= 30 && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * 90 / 100)
                            {
                                //progLog(" Word of Revival");
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[22].Cells[1].Value.ToString()), 0);
                                CoolDowns[0] = 0;
                                Thread.Sleep(1250);
                            }
                            if (Boolean.Parse(uChanter.UseAethericField.Checked.ToString()) && CoolDowns[7] >= 600)
                            {

                            }
                            while (!PlayerDead() && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * StayAbove / 100)
                            {
                                if (Boolean.Parse(uChanter.UseRecoverySpell.Checked.ToString()) && CoolDowns[5] >= 10)
                                {
                                    //progLog(" Recovery spell");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[21].Cells[1].Value.ToString()), 0);
                                    CoolDowns[5] = 0;
                                    Thread.Sleep(1000);
                                }
                                // Healing Light
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[20].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(400);
                                if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                {
                                    Thread.Sleep((int)oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime));
                                }
                                if (CoolDowns[2] >= 30)
                                {
                                    //progLog(" Focuseded parry"); 
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[23].Cells[1].Value.ToString()), 0);
                                    CoolDowns[2] = 0;
                                    Thread.Sleep(2000);
                                }
                                if (CoolDowns[1] >= 600 && oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * 40 / 100)
                                {
                                    //progLog(" Word of Quickness"); 
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                    CoolDowns[1] = 0;
                                    Thread.Sleep(2000);
                                }
                                if (CoolDowns[0] >= 30)
                                {
                                    //progLog(" Word of Revival");
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[22].Cells[1].Value.ToString()), 0);
                                    CoolDowns[0] = 0;
                                    Thread.Sleep(1250);
                                }
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);
                                Thread.Sleep(250);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.VK_C, 0);
                            }
                            if (Boolean.Parse(uChanter.UseProtectiveWard.Checked.ToString()))
                            {
                                if (!TargetIsDead())
                                {
                                    if (CoolDowns[8] >= 120)
                                    {
                                        //progLog(" Protective ward");
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[15].Cells[1].Value.ToString()), 0);
                                        CoolDowns[8] = 0;
                                        Thread.Sleep(2000);
                                    }
                                }
                                else break;
                            }
                        }
                    }
                    if (PlayerDead())
                        return "PlayerDead";
                    Kills += 1;
                    progLog(" Combat done");
                    this.BeginInvoke(new MethodInvoker(delegate
                    {

                        if (BeepOnKill.Checked)
                            SoundAlert("TargetDead");
                        //progLog(" Exp gained: " + string.Format("{0:#,#}", (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Exp) - Exp)));
                        ExpEarned_Label.Text = "Exp: " + string.Format("{0:#,#}", (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Exp) - Exp));
                        if (oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_Level) > pLevel)
                        {
                            Levels += 1;
                            Exp = 0;
                            pLevel = oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_Level);
                        }
                    }));
                    return "TargetDead";
                }
                return "PlayerDead";
            }
            return "";
        }

        // Target Functions
        public float TargetDistanceFromPlayer()
        {
            if (oMemory.OpenProcess(PID))
            {
                float X = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_X);
                float Y = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_Y);
                float tRange = (float)Math.Sqrt(X * X + Y * Y);
                return tRange;
            }
            return 0;
        }
        public bool TargetIsDead()
        {
            if (oMemory.OpenProcess(PID))
            {
                if (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool TargetIsCasting()
        {
            if (oMemory.OpenProcess(PID))
            {
                if (oMemory.ReadByte(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_TargetIsCasting) == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public string TargetType()
        {
            if (oMemory.OpenProcess(PID))
            {
                return oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Type, 6, false, 0);
            }
            return string.Empty;
        }
        public bool TargetMoving()
        {
            if (oMemory.OpenProcess(PID))
            {
                if (oMemory.ReadByte(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_TargetIsMoving) == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public bool TargetIsAppropriate()
        {
            if (oMemory.OpenProcess(PID))
            {
                // Is target attacking us?
                if (TargetIsTargetingPlayer())
                {
                    progLog(" Ambushed!");
                    return true;
                }
                // Check if target is blacklisted
                if (BlackListName[0] != null)
                {
                    for (int i = 0; i < BlackListName.Length; i++)
                    {
                        if (oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Name, 40, true, 0) == BlackListName[i])
                        {
                            progLog(" Target is blacklisted"); 
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                            return false;
                        }
                    }
                    return true;
                }
                // Target distance check
                if (TargetDistanceFromPlayer() > Int32.Parse(MaxTargetDistance.Text))
                {
                    progLog(" Target is too far way");
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                    return false;
                }
                // Check if targets level is too high
                if (!(oMemory.ReadByte(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Level) >= Int32.Parse(MaxTargetLevel.Text)))
                {
                    if (oMemory.ReadByte(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Level) == 1 || oMemory.ReadByte(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Level) == 2)
                    {
                        if (oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_Level) >= 6)
                        {
                            progLog(" Target level inappropriate");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                            return false;
                        }
                    }
                }
                else
                {
                    progLog(" Target level inappropriate"); 
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                    return false;
                }
                // Check if target is currently in combat
                if (TargetInCombat())
                {
                    if (Boolean.Parse(StealMobs.Checked.ToString()))
                    {
                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_HealthMax) * 20 / 100)
                        {
                            progLog(" Target inutile");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                            return false;
                        }
                        else
                        {
                            progLog(" Stealing target"); 
                            return true;
                        }
                    }
                    else
                    {
                        progLog(" Target is attacking another player"); 
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                        return false;
                    }
                }
                // Target is appropriate
                progLog(" Engaging target");
                return true;
            }
            return true;
        }
        public bool TargetInCombat()
        {
            if (oMemory.OpenProcess(PID))
            {
                if (oMemory.ReadByte(_Address_Base + nmLocation._Address_TargetInCombat, nmLocation.offsets_TargetInCombat) >= 1)
                {
                    return true;
                }
            }
            return false;
        }
        public bool TargetIsTargetingPlayer()
        {
            if (oMemory.OpenProcess(PID))
            {
                if (oMemory.ReadShort(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_TargetofTarget_ID) == oMemory.ReadShort(_Address_Base + nmLocation._Address_LocalPlayer_ID))
                {
                    return true;
                }
            }
            return false;
        }
        
        // Player Functions
        public string PlayerCheck(bool SkipGainMana)
        {
            if (oMemory.OpenProcess(PID))
            {
                if (PlayerDead())
                    return "PlayerDead";
                else if (TargetIsTargetingPlayer())
                    return "Ambushed";

                // HERB TREATMENT
                if (Boolean.Parse(Rest_UseHerbTreatment.Checked.ToString()))
                {
                    if (KeymapGrid.Rows[3].Cells[1].Value != null && KeymapGrid.Rows[3].Cells[1].Value.ToString() != string.Empty)
                    {
                        if (Boolean.Parse(Rest_UseHerbTreatmentHP.Checked.ToString()))
                        {
                            if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * Int32.Parse(Rest_UseHerbTreatmentValue.Text) / 100)
                            {
                                progLog(" Herb Treatment... "); 
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[3].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(500);
                                do
                                {
                                    if (PlayerDead())
                                        return "PlayerDead";
                                    else if (TargetIsTargetingPlayer())
                                        return "Ambushed";
                                    Thread.Sleep(250);
                                }
                                while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0);
                                Thread.Sleep(500);
                            }
                        }
                        if (Boolean.Parse(Rest_UseHerbTreatmentMP.Checked.ToString()))
                        {
                            if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Mana) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_ManaMax) * Int32.Parse(Rest_UseHerbTreatmentValue.Text) / 100)
                            {
                                progLog(" Mana Treatment... ");
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[3].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(500);
                                do
                                {
                                    if (PlayerDead())
                                        return "PlayerDead";
                                    else if (TargetIsTargetingPlayer())
                                        return "Ambushed";
                                    Thread.Sleep(250);
                                }
                                while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0);
                                Thread.Sleep(500);
                            }
                        }
                    }
                    else
                        progLog(" Error: No key set for Treatment"); 
                }
                // CLERIC
                if (CurrentClass == "Cleric")
                {
                    // DISPEL DEBUFFS
                    if (oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_DebffCount, nmLocation.offsets_Debuffcount) >= 1)
                    {
                        if (KeymapGrid.Rows[17].Cells[1].Value != null && KeymapGrid.Rows[17].Cells[1].Value.ToString() != string.Empty)
                        {
                            short TimeOut = 0;
                            progLog(" Dispelling debuffs"); 
                            do
                            {
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[17].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(1000);
                                TimeOut++;
                                if (TimeOut >= 5)
                                {
                                    progLog(" Error: Timed out during debuff dispel");
                                    break;
                                }
                            }
                            while (oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_DebffCount, nmLocation.offsets_Debuffcount) >= 1);
                        }
                        else
                           progLog(" Error: No key set for dispel"); 
                    }
                    // PENANCE
                    if (Boolean.Parse(uCleric.UsePenance.Checked.ToString()))
                    {
                        if (KeymapGrid.Rows[20].Cells[1].Value != null && KeymapGrid.Rows[20].Cells[1].Value.ToString() != string.Empty)
                        {
                            if (BuffTime[6] >= 181)
                            {
                                BuffTime[6] = 0;
                                progLog(" Using penance");
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[20].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(1400);
                            }
                        }
                        else
                            progLog(" Error: No key set for penance"); 
                    }
                    // HEAL LOOP
                    if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * Int32.Parse(Rest_Health.Text) / 100)
                    {
                        do
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(500);
                            if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                            {
                                //progLog(" Casting detected");
                                while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                                {
                                    Thread.Sleep(100);
                                }
                                Thread.Sleep(1400);
                            }
                            if (PlayerDead())
                                return "PlayerDead";
                            else if (TargetIsTargetingPlayer())
                                return "Ambushed";
                        }
                        while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * Int32.Parse(Rest_Health.Text) / 100);
                    }
                    // HEAL OVER TIME BUFFS
                    if (Boolean.Parse(uCleric.UseLightofRenewal.Checked.ToString()))
                    {
                        if (CoolDowns[0] >= 10)
                        {
                            CoolDowns[0] = 0;
                            //progLog(" Light of Renewal");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1400);
                        }
                    }
                    else
                    {
                        if (KeymapGrid.Rows[19].Cells[1].Value != null && KeymapGrid.Rows[19].Cells[1].Value.ToString() != string.Empty)
                        {
                            if (CoolDowns[0] >= 31)
                            {
                                CoolDowns[0] = 0;
                                //progLog(" Light of Renewal");
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[19].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(1400);
                            }
                        }
                        else
                            progLog(" Error: No key set for Light of Rejuvenation");
                    }
                    // SUBSTANTIAL BUFFS
                    if (BuffTime[4] >= 1800 || BuffTime[5] >= 3600)
                    {
                        progLog(" Rebuffing");
                        if (BuffTime[4] >= 1800)
                        {
                            BuffTime[4] = 0;
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1400);
                        }
                        if (BuffTime[5] >= 3600)
                        {
                            BuffTime[5] = 0;
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[9].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1400);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[10].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1400);
                        }
                    }
                }
                // CHANTER
                else if (CurrentClass == "Chanter")
                {
                    if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * Int32.Parse(Rest_Health.Text) / 100)
                    {
                        progLog(" Healing"); 
                        do
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[20].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(400);
                            if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime) > 0)
                            {
                                Thread.Sleep((int)oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_CastingTime));
                            }
                            if (PlayerDead())
                                return "PlayerDead";
                            else if (TargetIsTargetingPlayer())
                                return "Ambushed";
                        }
                        while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * Int32.Parse(Rest_Health.Text) / 100);
                        Thread.Sleep(2000);
                    }
                    if (BuffTime[4] >= 1800 || BuffTime[5] >= 3600)
                    {
                        progLog(" Rebuffing"); 
                        if (BuffTime[4] >= 1800)
                        {
                            if (Boolean.Parse(uChanter.UseRageSpell.Checked.ToString()))
                            {
                                Thread.Sleep(1750);
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0);
                            }
                            Thread.Sleep(1750);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[9].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(2250);
                            BuffTime[4] = 0;
                        }
                        if (BuffTime[5] >= 3600)
                        {
                            Thread.Sleep(1750);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[10].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1750);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[11].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(2250);
                            BuffTime[5] = 0;
                        }
                    }
                    if (Boolean.Parse(uChanter.UseWordofWind.Checked.ToString()) && CoolDowns[6] >= 1800 && oMemory.ReadShort(_Address_Base + nmLocation._Address_LocalPlayer_DP) >= 2000)
                    {
                        progLog(" Buffing Word of Wind"); 
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[27].Cells[1].Value.ToString()), 0);
                        CoolDowns[6] = 0;
                        Thread.Sleep(1400);
                    }
                    if (KeymapGrid.Rows[28].Cells[1].Value != null && KeymapGrid.Rows[28].Cells[1].Value.ToString() != string.Empty)
                    {
                        if (BuffTime[6] >= 121)
                        {
                            progLog(" Buffing Spell of Ascension");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[28].Cells[1].Value.ToString()), 0);
                            BuffTime[6] = 0;
                            Thread.Sleep(1400);
                        }
                    }
                }
                // SORCERER
                else if (CurrentClass == "Sorcerer")
                {
                    if (BuffTime[3] >= 60)
                    {
                        if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Mana) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_ManaMax) * 90 / 100)
                        {
                            if (KeymapGrid.Rows[12].Cells[1].Value != null && KeymapGrid.Rows[12].Cells[1].Value.ToString() != string.Empty)
                            {
                                BuffTime[3] = 0;
                                progLog(" Absorbing Energy..."); 
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(1500);
                            }
                            else
                                progLog(" Error: No key set for Absorb Energy...");
                        }
                    }
                    if (Boolean.Parse(uSorcerer.UseGainMana.Checked.ToString()))
                    {
                        if (!SkipGainMana)
                        {
                            if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Mana) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_ManaMax) * 80 / 100)
                            {
                                if (BuffTime[5] >= 300)
                                {
                                    if (KeymapGrid.Rows[21].Cells[1].Value != null && KeymapGrid.Rows[21].Cells[1].Value.ToString() != string.Empty)
                                    {
                                        BuffTime[5] = 0;
                                        progLog(" Gaining Mana..."); 
                                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[21].Cells[1].Value.ToString()), 0);
                                        Thread.Sleep(7000);
                                    }
                                    else
                                        progLog(" Error: No key set for Gain Mana..."); 
                                }
                            }
                        }
                    }
                    if (BuffTime[4] >= 1800)
                    {
                        if (KeymapGrid.Rows[14].Cells[1].Value != null && KeymapGrid.Rows[14].Cells[1].Value.ToString() != string.Empty)
                        {
                            BuffTime[4] = 0;
                            progLog(" Robe Buff...");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1400);
                        }
                        else
                            progLog(" Error: No key set for Robe Buff..."); 
                    }
                    if (BuffTime[6] >= 120)
                    {
                        if (KeymapGrid.Rows[13].Cells[1].Value != null && KeymapGrid.Rows[13].Cells[1].Value.ToString() != string.Empty)
                        {
                            BuffTime[6] = 0;
                            progLog(" Buff Stone Skin...");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1400);
                        }
                        else
                            progLog(" Error: No key set for Stone Skin..."); 
                    }
                }
                // RESTING
                if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * Int32.Parse(Rest_Health.Text) / 100 || oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Mana) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_ManaMax) * Int32.Parse(Rest_Mana.Text) / 100)
                {
                    if (oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_DebffCount, nmLocation.offsets_Debuffcount) >= 1)
                    {
                        progLog(" Debuff detected"); 
                        progLog(" Waiting... "); 
                        for (int i = 0; i < 65; i++)
                        {
                            if (i >= 60)
                                oMemory.Write(_Address_Base + nmLocation._Address_LocalPlayer_DebffCount, nmLocation.offsets_Debuffcount, 0);
                            if (oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_DebffCount, nmLocation.offsets_Debuffcount) == 0)
                                break;
                            if (PlayerDead())
                                return "PlayerDead";
                            else if (TargetIsTargetingPlayer())
                                return "Ambushed";
                            Thread.Sleep(500); 
                        }
                    }
                    progLog(" Resting..."); 
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.OEM_COMMA, 0);
                    Thread.Sleep(200);
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.OEM_COMMA, 0);
                    Thread.Sleep(200);
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.OEM_COMMA, 0);
                    do
                    {
                        if (PlayerDead())
                            return "PlayerDead";
                        else if (TargetIsTargetingPlayer())
                            return "Ambushed";
                        Thread.Sleep(500);
                    }
                    while (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * 95 / 100 || oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Mana) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_ManaMax) * 95 / 100);
                    progLog(" Rest done"); 
                    tRested += 1;
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.OEM_COMMA, 0);
                    Thread.Sleep(200);
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.OEM_COMMA, 0);
                    Thread.Sleep(1750);
                }
                // ASSASSIN
                if (CurrentClass == "Assassin")
                {
                    if (Boolean.Parse(uAssassin.UsePoison.Checked.ToString()))
                    {
                        if (BuffTime[6] >= 121)
                        {
                            if (KeymapGrid.Rows[15].Cells[1].Value != null && KeymapGrid.Rows[15].Cells[1].Value.ToString() != string.Empty)
                            {
                                BuffTime[6] = 0;
                                progLog(" Applying Poisons...");
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[15].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(1500);
                            }
                            else
                               progLog(" Error: No key set for Poisons...");
                        }
                    }
                    if (BuffTime[5] >= 121)
                    {
                        if (KeymapGrid.Rows[17].Cells[1].Value != null && KeymapGrid.Rows[17].Cells[1].Value.ToString() != string.Empty)
                        {
                            BuffTime[5] = 0;
                            progLog(" Buffing Clear Focus");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[17].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1800);
                        }
                        else
                            progLog(" Error: No key set for Clear Focus..."); 
                    }
                }
                // RANGER
                else if (CurrentClass == "Ranger")
                {
                    if (BuffTime[6] >= 61)
                    {
                        if (Boolean.Parse(uRanger.UseDodging.Checked.ToString()))
                        {
                            if (KeymapGrid.Rows[12].Cells[1].Value != null && KeymapGrid.Rows[12].Cells[1].Value.ToString() != string.Empty)
                            {
                                progLog(" Dodging..."); 
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                                BuffTime[6] = 0;
                                Thread.Sleep(1400);
                            }
                            else
                               progLog(" Error: No key set for Dodging!");
                        }
                        if (Boolean.Parse(uRanger.UseAiming.Checked.ToString()))
                        {
                            if (KeymapGrid.Rows[14].Cells[1].Value != null && KeymapGrid.Rows[14].Cells[1].Value.ToString() != string.Empty)
                            {
                                //progLog(" Aiming..."); 
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                                BuffTime[6] = 0;
                                Thread.Sleep(1400);
                            }
                            else
                               progLog(" Error: No key set for Aiming");
                        }
                        if (Boolean.Parse(uRanger.UseStrongShots.Checked.ToString()))
                        {
                            if (KeymapGrid.Rows[16].Cells[1].Value != null && KeymapGrid.Rows[16].Cells[1].Value.ToString() != string.Empty)
                            {
                                //progLog(" Strong Shots..."); 
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                BuffTime[6] = 0;
                                Thread.Sleep(1400);
                            }
                            else
                                progLog(" Error: No key set for Strong Shots"); 
                        }
                    }
                    if (oMemory.ReadShort(_Address_Base + nmLocation._Address_LocalPlayer_DP) >= 2000)
                    {
                        if (KeymapGrid.Rows[17].Cells[1].Value != null && KeymapGrid.Rows[17].Cells[1].Value.ToString() != string.Empty)
                        {
                            progLog(" Mau Form"); 
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[17].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1400);
                        }
                        else
                            progLog(" Error: No key set for Mau Form"); 
                    }
                }
                if (FirstBuff_cBox.Checked && BuffTime[0] >= int.Parse(FirstBuff_Duration.Text) * 60)
                {
                    if (KeymapGrid.Rows[5].Cells[1].Value != null && KeymapGrid.Rows[5].Cells[1].Value.ToString() != String.Empty)
                    {
                        BuffTime[0] = 0;
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[5].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(800);
                    }
                    else
                        progLog(" Error: No key set for Buff1");
                }
                if (SecondBuff_cbox.Checked && BuffTime[1] >= int.Parse(SecondBuff_Duration.Text) * 60)
                {
                    if (KeymapGrid.Rows[6].Cells[1].Value != null && KeymapGrid.Rows[6].Cells[1].Value.ToString() != String.Empty)
                    {
                        BuffTime[1] = 0;
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[6].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(800);
                    }
                    else
                        progLog(" Error: No key set for Buff2"); 
                }
                if (ThirdBuff_cbox.Checked && BuffTime[2] >= int.Parse(ThirdBuff_Duration.Text) * 60)
                {
                    if (KeymapGrid.Rows[7].Cells[1].Value != null && KeymapGrid.Rows[7].Cells[1].Value.ToString() != String.Empty)
                    {
                        BuffTime[2] = 0;
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[7].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(800);
                    }
                    else
                        progLog(" Error: No key set for Buff3");
                }
                if (PlayerDead())
                    return "PlayerDead";
                else if (TargetIsTargetingPlayer())
                    return "Ambushed";
                else
                    return "Ready";
            }
            return string.Empty;
        }
        public bool PlayerHealthBelowPercent(int Percent)
        {
            if (oMemory.OpenProcess(PID))
            {
                if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) < oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_HealthMax) * Percent / 100)
                {
                    return true;
                }
            }
            return false;
        }
        public bool PlayerMoving()
        {
            if (oMemory.OpenProcess(PID))
            {
                float pPositionY = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y);
                float pPositionX = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X);
                Thread.Sleep(1200);
                if (oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y) != pPositionY && oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) != pPositionX)
                {
                    return true;
                }
            }
            progLog(" Stuck detected");
            return false;
        }
        public short PlayerLevel()
        {
            if (oMemory.OpenProcess(PID))
            {
                return oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_Level);
            }
            return 0;
        }
        public bool PlayerDead()
        {
            if (oMemory.OpenProcess(PID))
            {
                if (oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Health) == 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool PlayerHasTarget()
        {
            if (oMemory.OpenProcess(PID))
            {
                if (oMemory.ReadByte(_Address_Base + nmLocation._Address_LocalPlayer_HasTarget) == 1)
                {
                    return true;
                }
            }
            return false;
        }       
        
        // Generic Functions
        public void Rebuff(bool IncludeExtraBuffs)
        {
            if (oMemory.OpenProcess(PID))
            {
                if (IncludeExtraBuffs)
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        if (FirstBuff_cBox.Checked)
                        {
                            FirstBuff_cBox.Checked = false;
                            FirstBuff_cBox.Checked = true;
                            Thread.Sleep(1000);
                        }
                        if (SecondBuff_cbox.Checked)
                        {
                            SecondBuff_cbox.Checked = false;
                            SecondBuff_cbox.Checked = true;
                            Thread.Sleep(1000);
                        }
                        if (ThirdBuff_cbox.Checked)
                        {
                            ThirdBuff_cbox.Checked = false;
                            ThirdBuff_cbox.Checked = true;
                            Thread.Sleep(1000);
                        }
                    }));
                }
                switch (CurrentClass)
                {
                    case "Assassin":
                        if (Boolean.Parse(uAssassin.UsePoison.Checked.ToString()))
                        {
                            if (KeymapGrid.Rows[15].Cells[1].Value != null && KeymapGrid.Rows[15].Cells[1].Value.ToString() != string.Empty)
                            {
                                BuffTime[6] = 0;
                                progLog(" Applying Poisons..."); 
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[15].Cells[1].Value.ToString()), 0);
                                Thread.Sleep(1500);
                            }
                            else
                                progLog(" Error: No key set for Poisons..."); 
                        }
                        if (KeymapGrid.Rows[17].Cells[1].Value != null && KeymapGrid.Rows[17].Cells[1].Value.ToString() != string.Empty)
                        {
                            BuffTime[5] = 0;
                            progLog(" Buffing Clear Focus");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[17].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1800);
                        }
                        else
                            progLog(" Error: No key set for Clear Focus..."); 
                        break;
                    case "Sorcerer":
                        if (KeymapGrid.Rows[14].Cells[1].Value != null && KeymapGrid.Rows[14].Cells[1].Value.ToString() != string.Empty)
                        {
                            BuffTime[4] = 0;
                            //progLog(" Buff Robe of Earth...");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1400);
                        }
                        if (KeymapGrid.Rows[13].Cells[1].Value != null && KeymapGrid.Rows[13].Cells[1].Value.ToString() != string.Empty)
                        {
                            BuffTime[6] = 0;
                            //progLog(" Buffing Stone Skin");
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                            Thread.Sleep(1400);
                        }
                        else
                            progLog(" Error: No key set for Stone Skin..."); 
                        break;
                    case "Ranger":
                        if (Boolean.Parse(uRanger.UseDodging.Checked.ToString()) || Boolean.Parse(uRanger.UseAiming.Checked.ToString()) || Boolean.Parse(uRanger.UseStrongShots.Checked.ToString()))
                        {
                            progLog(" Rebuffing"); 
                            if (Boolean.Parse(uRanger.UseAiming.Checked.ToString()))
                            {
                                if (KeymapGrid.Rows[12].Cells[1].Value != null && KeymapGrid.Rows[12].Cells[1].Value.ToString() != string.Empty)
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                                    BuffTime[6] = 0;
                                    Thread.Sleep(2000);
                                }
                                else
                                    progLog(" Error: No key set for aiming...");
                            }
                            if (Boolean.Parse(uRanger.UseDodging.Checked.ToString()))
                            {
                                if (KeymapGrid.Rows[14].Cells[1].Value != null && KeymapGrid.Rows[14].Cells[1].Value.ToString() != string.Empty)
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                                    BuffTime[6] = 0;
                                    Thread.Sleep(2000);
                                }
                                else
                                    progLog(" Error: No key set for dodging...");
                            }
                            if (Boolean.Parse(uRanger.UseStrongShots.Checked.ToString()))
                            {
                                if (KeymapGrid.Rows[16].Cells[1].Value != null && KeymapGrid.Rows[16].Cells[1].Value.ToString() != string.Empty)
                                {
                                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[16].Cells[1].Value.ToString()), 0);
                                    BuffTime[6] = 0;
                                    Thread.Sleep(2000);
                                }
                                else
                                    progLog(" Error: No key set for Strong Shots!");;
                            }
                        }
                        break;
                    case "Gladiator":
                        progLog(" Rebuffing");
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(1750);
                        break;
                    case "Cleric":
                        progLog(" Rebuffing");
                        Thread.Sleep(1000);

                        BuffTime[4] = 0;
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0); // 30 minute buff
                        Thread.Sleep(1400);

                        BuffTime[5] = 0;
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[9].Cells[1].Value.ToString()), 0); // 60 minute buff
                        Thread.Sleep(1400);
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[10].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(1400);
                        break;

                    case "Chanter":
                        progLog(" Rebuffing");
                        if (bool.Parse(uChanter.UseRageSpell.Checked.ToString()))
                        {
                            Thread.Sleep(1750);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[8].Cells[1].Value.ToString()), 0);
                        }
                        Thread.Sleep(1750);
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[9].Cells[1].Value.ToString()), 0);
                        BuffTime[4] = 0;

                        Thread.Sleep(1750);
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[10].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(1750);
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[11].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(1750);
                        BuffTime[5] = 0;

                        // Mantras
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[12].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(1750);
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[13].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(1750);
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[14].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(2000);
                        if (KeymapGrid.Rows[28].Cells[1].Value != null && KeymapGrid.Rows[28].Cells[1].Value.ToString() != string.Empty)
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[28].Cells[1].Value.ToString()), 0);
                            BuffTime[6] = 0;
                            Thread.Sleep(1500);
                        }
                        break;
                }
            }
        }
        public void Loot()
        {
            short TimeOutCount = 0;
            short TimeOut = 12;
            if (CurrentClass == "Ranger" || CurrentClass == "Sorcerer")
                TimeOut = 18;

            if (bool.Parse(SkipLoot.Checked.ToString()))
            {
                while (PlayerHasTarget())
                {
                    if (PlayerDead()) 
                        break;
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                    Thread.Sleep(500);
                }
                return;
            }
            else
            {
                progLog(" Looting...");
                do
                {
                    if (PlayerDead()) 
                        break;
                    if (KeymapGrid.Rows[2].Cells[1].Value != null && KeymapGrid.Rows[2].Cells[1].Value.ToString() != string.Empty)
                    {
                        PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[2].Cells[1].Value.ToString()), 0);
                        Thread.Sleep(500);
                    }
                    else
                    {
                        progLog(" Error: No loot key set!");
                        break;
                    }
                    /*
                    if (oMemory.ReadByte(_Address_Base + nmLocation.LootConfirmation, nmLocation.offsets_LootConfirm) != 255)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate { progLog(" Confirming loot"); }));
                        do
                        {
                            if (PlayerDead()) 
                                break;
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.RETURN, 0);
                            Thread.Sleep(1000);
                        }
                        while (oMemory.ReadByte(_Address_Base + nmLocation.LootConfirmation, nmLocation.offsets_LootConfirm) != 255);
                    } */
                    TimeOutCount++;
                    if (TimeOutCount >= TimeOut)
                    {
                        progLog(" Error looting... Cancelling"); 
                        while (PlayerHasTarget())
                        {
                            if (PlayerDead())
                                break;
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                            Thread.Sleep(500);
                        }
                    }
                }
                while (PlayerHasTarget() && TargetIsDead() && !PlayerDead());
                Thread.Sleep(1000);
            }
        }
        public bool RevivePlayer()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(delegate {

                        this.WindowState = FormWindowState.Normal;

                    }));
                }
                else
                    this.WindowState = FormWindowState.Normal;
            }
            SetForegroundWindow(this.Handle);
            Thread.Sleep(6000);
            Rectangle rect;
            GetWindowRect(child, out rect);
            short Y_Value = 365;
            short TimeOut = 0;
            do
            {
                SetForegroundWindow(child);
                MouseSimulator.X = rect.X + 482;
                MouseSimulator.Y = rect.Y + Y_Value;
                Thread.Sleep(25);
                MouseSimulator.Click(MouseButton.Left);
                Y_Value++;
                if (Y_Value >= 410)
                {
                    Y_Value = 365;
                    TimeOut++;
                    if (TimeOut >= 3)
                        progLog(" Failed to revive player");
                        return false;
                }
            }
            while (PlayerDead());
            Thread.Sleep(1500);
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(20);
                MouseSimulator.MouseWheel(-120);
            }
            if (oMemory.OpenProcess(PID))
                oMemory.Write(_Address_Base + nmLocation._Address_CameraX, 25.0f);
            Thread.Sleep(1000);
            return true;
        }
        public bool HealSoul()
        {
            Rectangle rect;
            GetWindowRect(child, out rect);
            if (SelectTargetByName(SoulHealerNPC.Text))
            {
                SetForegroundWindow(child);
                if (KeymapGrid.Rows[0].Cells[1].Value != null && KeymapGrid.Rows[0].Cells[1].Value.ToString() != string.Empty)
                {
                    Thread.Sleep(200);
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                    Thread.Sleep(200);
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                    Thread.Sleep(200);
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(KeymapGrid.Rows[0].Cells[1].Value.ToString()), 0);
                    Thread.Sleep(1000);
                    if (oMemory.OpenProcess(PID))
                    {
                        uint cExp = oMemory.ReadInt(_Address_Base + nmLocation._Address_LocalPlayer_Exp);
                        int pX = 225;
                        int cCount = 0;
                        while (oMemory.ReadByte(_Address_Base + nmLocation.DialogueBox_SoulHeal) == 0)
                        {
                            MouseSimulator.X = rect.X + 100;
                            MouseSimulator.Y = rect.Y + pX;
                            MouseSimulator.Click(MouseButton.Left);
                            Thread.Sleep(100);
                            pX++;
                            if (pX >= 255)
                            {
                                pX = 225;
                                cCount++;
                                if (cCount >= 3)
                                    return false;
                            }
                        }
                        if (oMemory.ReadByte(_Address_Base + nmLocation.DialogueBox_SoulHeal) != 0)
                        {
                            Thread.Sleep(500);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.RETURN, 0);
                            Thread.Sleep(500);
                            do
                            {
                                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.ESCAPE, 0);
                                Thread.Sleep(500);
                            }
                            while (PlayerHasTarget());
                        }
                    }
                }
                else
                    progLog(" No key set for open dialogue");
            }
            else
                progLog(" Couldn't target Soul healer");

            return true;
        }
        public void QuitGame()
        {
            string Text = "/Quit";
            progLog(" Exiting game... ");
            Thread.Sleep(2500);
            foreach (char tCHAR in Text)
            {
                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(tCHAR.ToString()), 0);
            }
            Thread.Sleep(500);
            PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.RETURN, 0);
        }
        public string ErrorScan()
        {
            if (oMemory.OpenProcess(PID))
            {
                if (oMemory.ReadString(nmLocation._Chat_ObjectBlocking, 120, true) == "You cannot use that because there is an obstacle in the way.")
                {
                    return "ObstacleBlocking";
                }
            }
            return null;
        }
        public bool SelectTargetByName(string NPC)
        {
            if (oMemory.OpenProcess(PID))
            {
                string Text = "/Select " + NPC;
                string tName = string.Empty;
                foreach (char tCHAR in Text)
                {
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(tCHAR.ToString()), 0);
                }
                Thread.Sleep(250);
                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.RETURN, 0);
                Thread.Sleep(500);
                foreach (char aChar in oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Name, 40, true, 0))
                {
                    if (char.IsLetter(aChar))
                        tName += aChar;
                    else
                        continue;
                }
                if (tName == NPC)
                    return true;
            }
            return false;
        }
        public int CubeSpace()
        {
            if (oMemory.OpenProcess(PID))
            {
                // When we haven't got the full cube expansion
                // The game counts these blocked out slots as already filled.
                return oMemory.ReadByte(_Address_Base + nmLocation._Address_Cube, nmLocation.offsets_MaxCubeSlots) - (oMemory.ReadByte(_Address_Base + nmLocation._Address_Cube, nmLocation.offsets_UsedCubeSlots) - 17);
            }
            else return 0;
        }

        // Healing Bot Functions
        public void FollowTarget()
        {
            if (oMemory.OpenProcess(PID))
            {
                string Text = "/follow";
                foreach (char tCHAR in Text)
                {
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, ToKeyCode(tCHAR.ToString()), 0);
                }
                Thread.Sleep(250);
                PostMessage(child, (int)VirtualKeyCode.WM_KEYDOWN, (int)VirtualKeyCode.RETURN, 0);
            }
        }
        public bool TargetIsHealingTarget()
        {
            string tName = null;

            if (oMemory.OpenProcess(PID))
            {
                foreach (char aChar in oMemory.ReadText(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offset_Target_Name, 25, true, 0))
                {
                    if (char.IsLetter(aChar))
                        tName += aChar;
                    else
                        continue;
                }
            }
            if (tName != null && tName.ToLower() == HealBotTarget.Text.ToLower())
                return true;

            return false;
        }

        // Waypoint Functions
        public void RotateCameraToTarget()
        {
            double result = 0;
            if (oMemory.OpenProcess(PID))
            {
                if (oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_X) < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_Y) > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    result = Math.Atan((oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_Y) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_X) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 - 90;
                    float tResult = (float)result;
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tResult);
                    return;
                }
                if (oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_X) < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_Y) < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    result = Math.Atan((oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_Y) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_X) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 - 90;
                    float tResult = (float)result;
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tResult);
                    return;
                }
                if (oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_X) > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_Y) > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    result = Math.Atan((oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_Y) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_X) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 + 90;
                    float tResult = (float)result;
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tResult);
                    return;
                }
                if (oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_X) > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_Y) < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    result = Math.Atan((oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_Y) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (oMemory.ReadFloat(_Address_Base + nmLocation._Address_Target_Ptr, nmLocation.offsets_Target_X) - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 + 90;
                    float tResult = (float)result;
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tResult);
                    return;
                }
            }
        }
        public string MoveToNextWaypoint()
        {
            int varTurn = 20;
            float tAngle;
            short indStuck = 0;
            if (oMemory.OpenProcess(PID))
            {
                // _|_
                // *|       
                if (WaypointsX[MoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && WaypointsY[MoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    tAngle = (float)(Math.Atan((WaypointsY[MoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (WaypointsX[MoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 - 90);
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                    do
                    {
                        if (TargetIsTargetingPlayer())
                            return "Ambushed";
                        if (!PlayerMoving())
                        {
                            Stuck++;
                            indStuck++;
                            if (indStuck >= 4)
                                varTurn = -40;

                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(200);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.DIVIDE, 0);
                            Thread.Sleep(1500);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle + varTurn);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(2000);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                        }
                    }
                    while (WaypointsX[MoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && WaypointsY[MoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y));
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                    MoveCountF++;
                    return "Ok";
                }
                // _|_
                //  |*
                if (WaypointsX[MoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && WaypointsY[MoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    tAngle = (float)(Math.Atan((WaypointsY[MoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (WaypointsX[MoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 - 90);
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                    do
                    {
                        if (TargetIsTargetingPlayer())
                            return "Ambushed";
                        if (!PlayerMoving())
                        {
                            Stuck++;
                            indStuck++;
                            if (indStuck >= 4)
                                varTurn = -40;
                            
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(200);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.DIVIDE, 0);
                            Thread.Sleep(1500);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle + varTurn);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(2000);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                        }
                    }
                    while (WaypointsX[MoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && WaypointsY[MoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y));
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                    MoveCountF++;
                    return "Ok";
                }
                // *|_
                //  |  
                if (WaypointsX[MoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && WaypointsY[MoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    tAngle = (float)(Math.Atan((WaypointsY[MoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (WaypointsX[MoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 + 90);
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                    do
                    {
                        if (TargetIsTargetingPlayer())
                            return "Ambushed";
                        if (!PlayerMoving())
                        {
                            Stuck++;
                            indStuck++;
                            if (indStuck >= 4)
                                varTurn = -40;
                            
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(200);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.DIVIDE, 0);
                            Thread.Sleep(1500);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle + varTurn);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(2000);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                        }
                    }
                    while (WaypointsX[MoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && WaypointsY[MoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y));
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                    MoveCountF++;
                    return "Ok";
                }
                // _|*
                //  | 
                if (WaypointsX[MoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && WaypointsY[MoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    tAngle = (float)(Math.Atan((WaypointsY[MoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (WaypointsX[MoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 + 90);
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                    do
                    {
                        if (TargetIsTargetingPlayer())
                            return "Ambushed";
                        if (!PlayerMoving())
                        {
                            Stuck++;
                            indStuck++;
                            if (indStuck >= 4)
                                varTurn = -40;
                            
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(200);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.DIVIDE, 0);
                            Thread.Sleep(1500);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle + varTurn);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(2000);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                        }
                    }
                    while (WaypointsX[MoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && WaypointsY[MoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y));
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                    MoveCountF++;
                    return "Ok";
                }
            }
            return string.Empty;
        }
        public string MoveToHuntingArea()
        {
            short varTurn = 200;
            float tAngle = 0;
            if (oMemory.OpenProcess(PID))
            {
                // _|_
                // *|       
                if (dWaypointsX[dMoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && dWaypointsY[dMoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    tAngle = (float)(Math.Atan((dWaypointsY[dMoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (dWaypointsX[dMoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 - 90);
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                    do
                    {
                        if (TargetIsTargetingPlayer())
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            return "Ambushed";
                        }
                        if (!PlayerMoving())
                        {
                            Stuck++;
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(200);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.DIVIDE, 0);
                            Thread.Sleep(1500);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle + varTurn);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(2000);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                        }
                    }
                    while (dWaypointsX[dMoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && dWaypointsY[dMoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y));
                    dMoveCountF++;
                    return "Ok";
                }
                // _|_
                //  |*
                if (dWaypointsX[dMoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && dWaypointsY[dMoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    tAngle = (float)(Math.Atan((dWaypointsY[dMoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (dWaypointsX[dMoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 - 90);
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                    do
                    {
                        if (TargetIsTargetingPlayer())
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            return "Ambushed";
                        }
                        if (!PlayerMoving())
                        {
                            Stuck++;
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(200);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.DIVIDE, 0);
                            Thread.Sleep(1500);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle + varTurn);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(2000);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                        }
                    }
                    while (dWaypointsX[dMoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && dWaypointsY[dMoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y));
                    dMoveCountF++;
                    return "Ok";
                }
                // *|_
                //  |  
                if (dWaypointsX[dMoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && dWaypointsY[dMoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    tAngle = (float)(Math.Atan((dWaypointsY[dMoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (dWaypointsX[dMoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 + 90);
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                    do
                    {
                        if (TargetIsTargetingPlayer())
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            return "Ambushed";
                        }
                        if (!PlayerMoving())
                        {
                            Stuck++;
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(200);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.DIVIDE, 0);
                            Thread.Sleep(1500);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle + varTurn);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(2000);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                        }
                    }
                    while (dWaypointsX[dMoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && dWaypointsY[dMoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y));
                    dMoveCountF++;
                    return "Ok";
                }
                // _|*
                //  | 
                if (dWaypointsX[dMoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && dWaypointsY[dMoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y))
                {
                    tAngle = (float)(Math.Atan((dWaypointsY[dMoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y)) / (dWaypointsX[dMoveCount] - oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X))) / 6.28 * 360 + 90);
                    oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                    do
                    {
                        if (TargetIsTargetingPlayer())
                        {
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            return "Ambushed";
                        }
                        if (!PlayerMoving())
                        {
                            Stuck++;
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(200);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.DIVIDE, 0);
                            Thread.Sleep(1500);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle + varTurn);
                            PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                            Thread.Sleep(2000);
                            oMemory.Write(_Address_Base + nmLocation._Address_CameraY, tAngle);
                        }
                    }
                    while (dWaypointsX[dMoveCount] > oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X) && dWaypointsY[dMoveCount] < oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y));
                    dMoveCountF++;
                    return "Ok";
                }
            }
            return "";
        }
        public bool FindClosestWaypoint()
        {
            if (oMemory.OpenProcess(PID))
            {
                short Range = 4;
                bool WaypointFound = false;
                float PlayerX = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_X);
                float PlayerY = oMemory.ReadFloat(_Address_Base + nmLocation._Address_LocalPlayer_Y);
                while (!WaypointFound)
                {
                    for (short i = 0; i < WaypointCount; i++)
                    {
                        if (WaypointsX[i] < PlayerX && WaypointsY[i] < PlayerY)
                        {
                            if (PlayerX - WaypointsX[i] <= Range && PlayerY - WaypointsY[i] <= Range)
                            {
                                //MessageBox.Show("Nearest X waypoint: " + WaypointsX[i].ToString() + " Nearest Y waypoint: " + WaypointsY[i].ToString() + "   Count: " + WaypointCount.ToString());
                                MoveCount = i;
                                return true;
                            }
                        }
                        if (WaypointsX[i] < PlayerX && WaypointsY[i] > PlayerY)
                        {
                            if (PlayerX - WaypointsX[i] <= Range && WaypointsY[i] - PlayerY <= Range)
                            {
                                //MessageBox.Show("Nearest X waypoint: " + WaypointsX[i].ToString() + " Nearest Y waypoint: " + WaypointsY[i].ToString() + "   Count: " + WaypointCount.ToString());
                                MoveCount = i;
                                return true;
                            }
                        }
                        if (WaypointsX[i] > PlayerX && WaypointsY[i] > PlayerY)
                        {
                            if (WaypointsX[i] - PlayerX <= Range && WaypointsY[i] - PlayerY <= Range)
                            {
                                //MessageBox.Show("Nearest X waypoint: " + WaypointsX[i].ToString() + " Nearest Y waypoint: " + WaypointsY[i].ToString() + "   Count: " + WaypointCount.ToString());
                                MoveCount = i;
                                return true;
                            }
                        }
                        if (WaypointsX[i] > PlayerX && WaypointsY[i] < PlayerY)
                        {
                            if (WaypointsX[i] - PlayerX <= Range && PlayerY - WaypointsY[i] <= Range)
                            {
                                //MessageBox.Show("Nearest X waypoint: " + WaypointsX[i].ToString() + " Nearest Y waypoint: " + WaypointsY[i].ToString() + "   Count: " + WaypointCount.ToString());
                                MoveCount = i;
                                return true;
                            }
                        }
                    }
                    Range++;
                    if (Range >= 20)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
        public short MoveCountF
        {
            // This property updates the waypoint MoveCount variable and exercises relevant functions
            get { return MoveCount; }
            set
            {
                MoveCount = value;
                this.BeginInvoke(new MethodInvoker(delegate { movecount_Label.Text = "Current: " + MoveCount.ToString(); }));
                if (MoveCount >= WaypointCount)
                    MoveCount = 0;
            }
        }
        public short dMoveCountF
        {
            get { return dMoveCount; }
            set
            {
                dMoveCount = value;
                if (dMoveCount >= dWaypointCount)
                {
                    PostMessage(child, (int)VirtualKeyCode.WM_KEYUP, (int)VirtualKeyCode.NUMLOCK, 0);
                    dMoveCount = 0;
                    FinishedDeathRun = true;
                }
            }
        }

        // Other
        public void progLog(string umsg)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    ProgramLog.Items.Add("["+DateTime.Now.ToLongTimeString()+"] " + umsg);
                    ProgramLog.TopIndex = ProgramLog.Items.Count - 1;
                }));
            }
            else
            {
                ProgramLog.Items.Add("[" + DateTime.Now.ToLongTimeString() + "] " + umsg);
                ProgramLog.TopIndex = ProgramLog.Items.Count - 1;
            }
        }
        public int ToKeyCode(String Key)
        {
            // Function for converting Characters to their respective Keycodes
            if (Key.Length > 1)
            {
                switch (Key)
                {
                    case "F1": return (int)VirtualKeyCode.F1;
                    case "F2": return (int)VirtualKeyCode.F2;
                    case "F3": return (int)VirtualKeyCode.F3;
                    case "F4": return (int)VirtualKeyCode.F4;
                    case "F5": return (int)VirtualKeyCode.F5;
                    case "F6": return (int)VirtualKeyCode.F6;
                    case "F7": return (int)VirtualKeyCode.F7;
                    case "F8": return (int)VirtualKeyCode.F8;
                    case "F9": return (int)VirtualKeyCode.F9;
                    case "F10": return (int)VirtualKeyCode.F10;
                    case "F11": return (int)VirtualKeyCode.F11;
                    case "F12": return (int)VirtualKeyCode.F12;

                    case "D1": return (int)VirtualKeyCode.VK_1;
                    case "D2": return (int)VirtualKeyCode.VK_2;
                    case "D3": return (int)VirtualKeyCode.VK_3;
                    case "D4": return (int)VirtualKeyCode.VK_4;
                    case "D5": return (int)VirtualKeyCode.VK_5;
                    case "D6": return (int)VirtualKeyCode.VK_6;
                    case "D7": return (int)VirtualKeyCode.VK_7;
                    case "D8": return (int)VirtualKeyCode.VK_8;
                    case "D9": return (int)VirtualKeyCode.VK_9;
                    case "D0": return (int)VirtualKeyCode.VK_0;
                }
            }
            else
            {
                switch (Convert.ToChar(Key.ToLower()))
                {
                    case ' ': return (int)VirtualKeyCode.SPACE;
                    case '/': return (int)VirtualKeyCode.OEM_2;
                    case '-': return (int)VirtualKeyCode.OEM_MINUS;
                    case '=': return (int)VirtualKeyCode.OEM_PLUS;

                    case 'a': return (int)VirtualKeyCode.VK_A;
                    case 'b': return (int)VirtualKeyCode.VK_B;
                    case 'c': return (int)VirtualKeyCode.VK_C;
                    case 'd': return (int)VirtualKeyCode.VK_D;
                    case 'e': return (int)VirtualKeyCode.VK_E;
                    case 'f': return (int)VirtualKeyCode.VK_F;
                    case 'g': return (int)VirtualKeyCode.VK_G;
                    case 'h': return (int)VirtualKeyCode.VK_H;
                    case 'i': return (int)VirtualKeyCode.VK_I;
                    case 'j': return (int)VirtualKeyCode.VK_J;
                    case 'k': return (int)VirtualKeyCode.VK_K;
                    case 'l': return (int)VirtualKeyCode.VK_L;
                    case 'm': return (int)VirtualKeyCode.VK_M;
                    case 'n': return (int)VirtualKeyCode.VK_N;
                    case 'o': return (int)VirtualKeyCode.VK_O;
                    case 'p': return (int)VirtualKeyCode.VK_P;
                    case 'q': return (int)VirtualKeyCode.VK_Q;
                    case 'r': return (int)VirtualKeyCode.VK_R;
                    case 's': return (int)VirtualKeyCode.VK_S;
                    case 't': return (int)VirtualKeyCode.VK_T;
                    case 'u': return (int)VirtualKeyCode.VK_U;
                    case 'v': return (int)VirtualKeyCode.VK_V;
                    case 'w': return (int)VirtualKeyCode.VK_W;
                    case 'x': return (int)VirtualKeyCode.VK_X;
                    case 'y': return (int)VirtualKeyCode.VK_Y;
                    case 'z': return (int)VirtualKeyCode.VK_Z;
                }
            }
            return 0;
        }
        private void SoundAlert(string umsg)
        {   
            // Sounds alerts via WAV files
            System.Media.SoundPlayer SoundNotification = new System.Media.SoundPlayer();
            switch (umsg)
            {
                case "TargetDead":
                    String tdWav = Application.StartupPath + "\\" + "tDied.wav";
                    if (System.IO.File.Exists(tdWav))
                    {
                        SoundNotification.SoundLocation = tdWav;
                        SoundNotification.Play();
                    }
                    break;

                case "PlayerDead":
                    String dWav = Application.StartupPath + "\\" + "Died.wav";
                    if (System.IO.File.Exists(dWav))
                    {
                        SoundNotification.SoundLocation = dWav;
                        SoundNotification.Play();
                    }
                    break;

                case "Whisper":
                    break;
            }
        }

        // Form closing stuff
        private void PlayForMe_FormClosing(object sender, FormClosingEventArgs e)
        {
            progLog(" Exiting form");
            if (CurrentClass == "Starter" || string.IsNullOrWhiteSpace(CurrentClass))
                return;
            if (AION.Length >= 1)
            {
                progLog(" Saving configs");
                String regPath = Application.StartupPath + "\\" + StatusPlayer_gBox.Text;
                System.IO.Directory.CreateDirectory(regPath);
                // SAVE OUTPUT LOG                                                                                     
                using (System.IO.TextWriter log = new System.IO.StreamWriter(regPath + "\\Out.log"))
                {
                    for (int i = 0; i < ProgramLog.Items.Count; i++)
                    {
                        log.WriteLine(ProgramLog.Items[i]);
                    }
                }
                // SAVE KEYMAP
                using (System.IO.TextWriter keyLog = new System.IO.StreamWriter(regPath + "\\keymap.txt"))
                {
                    for (int i = 0; i < KeymapGrid.Rows.Count; i++)
                    {
                        keyLog.WriteLine(KeymapGrid.Rows[i].Cells[0].Value + " " + KeymapGrid.Rows[i].Cells[1].Value);
                    }
                }
                // SAVE CFG FILE
                String cfgPath = regPath + "\\" + StatusPlayer_gBox.Text + ".cfg";
                using (System.IO.TextWriter cfg = new System.IO.StreamWriter(cfgPath))
                {
                    Control fClass = null;
                    int OpenSkillIndex = 0;
                    switch (CurrentClass)
                    {
                        case "Ranger":
                            cfg.WriteLine("[RANGER]");
                            fClass = uRanger;
                            OpenSkillIndex = uRanger.OpenSkill.SelectedIndex;
                            break;
                        case "Gladiator":
                            cfg.WriteLine("[GLADIATOR]");
                            fClass = uGladiator;
                            OpenSkillIndex = uGladiator.OpenSkill.SelectedIndex;
                            break;
                        case "Assassin":
                            cfg.WriteLine("[ASSASSIN]");
                            fClass = uAssassin;
                            OpenSkillIndex = uAssassin.OpenSkill.SelectedIndex;
                            break;
                        case "Cleric":
                            cfg.WriteLine("[CLERIC]");
                            fClass = uCleric;
                            OpenSkillIndex = uCleric.OpenSkill.SelectedIndex;
                            break;
                        case "Chanter":
                            cfg.WriteLine("[CHANTER]");
                            fClass = uChanter;
                            OpenSkillIndex = uChanter.OpenSkill.SelectedIndex;
                            break;
                        case "Sorcerer":
                            cfg.WriteLine("[SORCERER]");
                            fClass = uSorcerer;
                            OpenSkillIndex = uSorcerer.OpenSkill.SelectedIndex;
                            break;
                    }
                    foreach (Control aControl in fClass.Controls)
                    {
                        if (aControl is Label)
                            continue;
                        if (aControl is ComboBox)
                            cfg.WriteLine("OpenSkill " + OpenSkillIndex);
                        if (aControl is CheckBox)
                        {
                            CheckBox State = (CheckBox)aControl;
                            if (State.Checked)
                                cfg.WriteLine(aControl.Name + " True");
                            else
                                cfg.WriteLine(aControl.Name + " False");
                        }
                    }
                    cfg.WriteLine("[GENERAL]");
                    cfg.WriteLine(MinimizeMode.Name + " " + MinimizeMode.Checked);
                    cfg.WriteLine(StealMobs.Name + " " + StealMobs.Checked);
                    cfg.WriteLine(ChaseRunners.Name + " " + ChaseRunners.Checked);
                    cfg.WriteLine(BeepOnKill.Name + " " + BeepOnKill.Checked);
                    cfg.WriteLine(BeepOnDeath.Name + " " + BeepOnDeath.Checked);
                    cfg.WriteLine(SkipLoot.Name + " " + SkipLoot.Checked);
                    cfg.WriteLine(StopLootingInventFull.Name + " " + StopLootingInventFull.Checked);

                    cfg.WriteLine("[LIMITS]");
                    cfg.WriteLine(Rest_Health.Name + " " + Rest_Health.Text);
                    cfg.WriteLine(Rest_Mana.Name + " " + Rest_Mana.Text);

                    cfg.WriteLine(Rest_UseHerbTreatment.Name + " " + Rest_UseHerbTreatment.Checked);
                    cfg.WriteLine(Rest_UseHerbTreatmentValue.Name + " " + Rest_UseHerbTreatmentValue.Text);
                    cfg.WriteLine(Rest_UseHealthPotion.Name + " " + Rest_UseHealthPotion.Checked);
                    cfg.WriteLine(Rest_UseManaPotion.Name + " " + Rest_UseManaPotion.Checked);
                    cfg.WriteLine(Rest_UsePotionValue.Name + " " + Rest_UsePotionValue.Text);
                    cfg.WriteLine(FirstBuff_Duration.Name + " " + FirstBuff_Duration.Text);
                    cfg.WriteLine(SecondBuff_Duration.Name + " " + SecondBuff_Duration.Text);
                    cfg.WriteLine(ThirdBuff_Duration.Name + " " + ThirdBuff_Duration.Text);

                    if (CurrentClass == "Chanter" || CurrentClass == "Cleric")
                        cfg.WriteLine(StayAboveHealth.Name + " " + StayAboveHealth.Text);

                    cfg.WriteLine("[PROFILE]");
                    cfg.WriteLine(WaypointFile_Label.Text);
                }
                if (!EmbedAION.Enabled)
                {
                    progLog(" Releasing client");
                    ReleaseAION.PerformClick();
                    Thread.Sleep(1000);
                }
            }
        }
        private void PlayForMe_MinimumSizeChanged(object sender, EventArgs e)
        {
            if (this.Visible)
                this.Hide();
            else
                this.Show();
        }

        // DLL imports
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void Grievous_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.End:
                    ReleaseAION.PerformClick();
                    break;
            }
        }
    }      
}