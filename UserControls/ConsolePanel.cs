﻿using System;
using System.IO;
using SLIL.Classes;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SLIL.UserControls
{
    internal partial class ConsolePanel : UserControl
    {
        private static bool ImHonest = false;
        private int CheatIndex = 0, ColorIndex = 0;
        private readonly Effect[] Effects =
        {
            new Regeneration(), new Adrenaline(),
            new Protection(), new Fatigue(),
            new Rider(), new Bleeding(),
            new Blindness(), new Stunned(),
            new VoidE(), new God()
        };
        private readonly List<string> History = new List<string>();
        internal List<Entity> Entities;
        internal Player player;
        internal string Command = "";
        private readonly Dictionary<string, Color> ColorMap = new Dictionary<string, Color>
        {
            { "-", Color.Yellow },
            { "*", Color.Tomato },
            { "~", Color.Cyan },
            { "<", Color.White }
        };
        private readonly Color[] ForeColors = 
        {
            Color.Lime, Color.White, Color.Magenta, 
            Color.Teal,Color.DeepSkyBlue, Color.SlateGray, 
            Color.Violet, Color.SandyBrown, Color.SpringGreen, 
            Color.Aquamarine, Color.Sienna
        };

        internal ConsolePanel()
        {
            InitializeComponent();
            Cursor = Program.SLILCursorDefault;
        }

        private void GetItem(int index) => player.DisposableItems[index].AddItem(99);

        private void Console_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
            {
                if (console.Text[console.Text.Length - 2] == ':' && console.Text[console.Text.Length - 1] == ' ') return;
                if (Command.Length > 0)
                {
                    int start = Command.Length - 1;
                    if (start < 0) start = 0;
                    Command = Command.Remove(start);
                }
                ConsoleDeleteText(1);
                return;
            }
        }

        private void Console_KeyUp(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            e.Handled = true;
            if (e.KeyCode == Keys.Enter && Command.Length > 0) DidCommand();
            else if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down)
            {
                if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                    return;
                char c = (char)e.KeyValue;
                if (!char.IsLetterOrDigit(c) && e.KeyCode != Keys.OemMinus && e.KeyCode != Keys.OemPeriod && e.KeyCode != Keys.Oemcomma && e.KeyCode != Keys.Space)
                    return;
                if (e.KeyCode == Keys.OemMinus) c = '_';
                else if (e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Oemcomma) c = ',';
                else if (e.KeyCode == Keys.Space) c = ' ';
                else if (e.KeyCode.ToString().StartsWith("Oem") || e.KeyCode == Keys.Divide || e.KeyCode == Keys.Subtract || e.KeyCode == Keys.Add)
                    return;
                Command += c.ToString();
                ConsoleAppendColoredText(c.ToString(), Color.Cyan);
            }
            if (e.KeyCode == Keys.Up)
            {
                if (History.Count == 0) return;
                ClearCommand();
                Command = History[CheatIndex];
                CheatIndex--;
                if (CheatIndex < 0)
                    CheatIndex = History.Count - 1;
                ConsoleAppendColoredText(Command, Color.Cyan);
            }
            if (e.KeyCode == Keys.Down)
            {
                if (History.Count == 0) return;
                ClearCommand();
                Command = History[CheatIndex];
                CheatIndex++;
                if (CheatIndex >= History.Count)
                    CheatIndex = 0;
                ConsoleAppendColoredText(Command, Color.Cyan);
            }
        }

        private void DidCommand()
        {
            SLIL parent = (SLIL)Parent.FindForm();
            Color color = ForeColors[ColorIndex];
            bool show_date = true, show_message = true;
            string message = null, time = null;
            string cheat = Command.ToUpper().Trim(' ').Replace("`", null);
            Command = "";
            if (cheat == "HELP")
            {
                show_date = false;
                message = "\n" +
                     "~┌─────────────┬─────────────────────────────────────────────┐~\n" +
                     "~│~ *Command*     ~│~ *Description*                                 ~│~\n" +
                     "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                     "~│~ -IMHONEST-    ~│~ Disable cheats                              ~│~\n" +
                     "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                     "~│~ -DEBUG-       ~│~ Go to debug map                             ~│~\n" +
                     "~│~ -DUMMY-       ~│~ Spawn a dummy                               ~│~\n" +
                     "~│~ -DEBUG_BOSS-  ~│~ Go to boss debug map                        ~│~\n" +
                     "~│~ -DEBUG_BIKE-  ~│~ Go to bike debug map                        ~│~\n" +
                     "~│~ -DEBUG_SPEED- ~│~ Show/hide debug player speed                ~│~\n" +
                     "~│~ -DEBUG_POS-   ~│~ Show/hide debug player position             ~│~\n" +
                     "~│~ -DEBUG_GAME-  ~│~ Show/hide game debug state                  ~│~\n" +
                     "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                     "~│~ -FPS-         ~│~ Show/hide FPS                               ~│~\n" +
                     "~│~ -MINIMAP-     ~│~ Show/hide Minimap                           ~│~\n" +
                     "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                     "~│~ -MVOL_-*X*      ~│~ Change music volume of sounds to X          ~│~\n" +
                     "~│~ -EVOL_-*X*      ~│~ Change effects volume of sounds to X        ~│~\n" +
                     "~│~ -VOL_-*X*       ~│~ Change volume of sounds to X                ~│~\n" +
                     "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                     "~│~ -CLS-         ~│~ Clearing the console                        ~│~\n" +
                     "~│~ -SLC-         ~│~ Clear console history                       ~│~\n" +
                     "~│~ -COLOR_-*X*     ~│~ Change console font color                   ~│~\n" +
                     "~│~ -SCOPE_-*X*     ~│~ Replace current sight                       ~│~\n" +
                     "~│~ -SCOPECOL_-*X*  ~│~ Change sight color                          ~│~\n" +
                     "~│~ -LOOK_-*X*      ~│~ Change mouse sensitivity                    ~│~\n" +
                     "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                     "~│~ -PLAYER-      ~│~ View player information                     ~│~\n" +
                     "~│~ -GUNS-        ~│~ Viewing weapon parameters                   ~│~\n" +
                     "~│~ -ENEMIES-     ~│~ View list of enemies                        ~│~\n" +
                     "~│~ -OSTS-        ~│~ View a list of game background music        ~│~\n" +
                     "~│~ -CHEATS-      ~│~ View list of cheats                         ~│~\n" +
                     "~└─────────────┴─────────────────────────────────────────────┘~";

            }
            else if (cheat == "DEBUG_SPEED")
            {
                parent.ShowDebugSpeed = !parent.ShowDebugSpeed;
                if (parent.ShowDebugSpeed)
                    message = "Display debug player speed enabled";
                else
                    message = "Display debug player speed disabled";
            }
            else if (cheat == "DEBUG_POS")
            {
                parent.ShowPositongDebug = !parent.ShowPositongDebug;
                if (parent.ShowPositongDebug)
                    message = "Display debug player position enabled";
                else
                    message = "Display debug player position disabled";
            }
            else if (cheat == "DEBUG_GAME")
            {
                parent.ShowGameDebug = !parent.ShowGameDebug;
                if (parent.ShowGameDebug)
                    message = "Display game debug state enabled";
                else
                    message = "Display game debug state disabled";
            }
            else if (cheat == "DEBUG")
            {
                show_message = false;
                SLIL.GoDebug(parent, 1);
            }
            else if (cheat == "DEBUG_BOSS")
            {
                show_message = false;
                SLIL.GoDebug(parent, 2);
            }
            else if (cheat == "DEBUG_BIKE")
            {
                show_message = false;
                SLIL.GoDebug(parent, 3);
            }
            else if (cheat == "CHEATS")
            {
                if (!ImHonest)
                {
                    show_date = false;
                    message = "\n" +
                         "~┌─────────────┬─────────────────────────────────────────────┐~\n" +
                         "~│~ *Command*     ~│~ *Description*                                 ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -IMHONEST-    ~│~ Disable cheats                              ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -CHSCH_-*X*     ~│~ Set the probability of cursed treatment     ~│~\n" +
                         "~│~ -DKSCH_-*X*     ~│~ Set the probability of a cursed kick        ~│~\n" +
                         "~│~ -MONEY_-*X*     ~│~ Change the amount of money to X             ~│~\n" +
                         "~│~ -SOTLG-       ~│~ Maximum amount of money                     ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -STAMIN_-*X*    ~│~ Changing player maximum stamina             ~│~\n" +
                         "~│~ -SPEED_-*X*     ~│~ Changing player movement speed              ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -BIGGUY-      ~│~ Give out \"The Smallest Pistol in the World\" ~│~\n" +
                         "~│~ -YHRII-       ~│~ Give \"Fingershot\"                           ~│~\n" +
                         "~│~ -IMGNOME-     ~│~ Give \"Wizard Gnome\"                         ~│~\n" +
                         "~│~ -ILLKLURDOG-  ~│~ Give \"Petition\"                             ~│~\n" +
                         "~│~ -COMEGETSOME- ~│~ Give \"Minigun\"                              ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -CAT-         ~│~ Give a pet: \"Silly cat\"                     ~│~\n" +
                         "~│~ -GNOME-       ~│~ Give a pet: \"Wizard Gnome\"                  ~│~\n" +
                         "~│~ -ENERGY-      ~│~ Give a pet: \"Energy Drink\"                  ~│~\n" +
                         "~│~ -ILOVEFURRY-  ~│~ Give a pet: \"Podseratel\"                    ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -BEFWK-       ~│~ Give out all weapons                        ~│~\n" +
                         "~│~ -FYTLG-       ~│~ Maximum amount of ammunition                ~│~\n" +
                         "~│~ -IDDQD-       ~│~ Upgrade all weapons by one level            ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -EFFECTS-     ~│~ List of effects                             ~│~\n" +
                         "~│~ -EFALLGV-     ~│~ Give all effects                            ~│~\n" +
                         "~│~ -EFCLEAR-     ~│~ Cleaning up effects                         ~│~\n" +
                         "~│~ -EFGIVE_-*X*    ~│~ Give effect under X id                      ~│~\n" +
                         "~│~ -EFGINF_-*X*    ~│~ Give infinite effect under X id             ~│~\n" +
                         "~│~ -EFG_-*X*-_TM_-*Y*  ~│~ Give effect under X id for Y seconds        ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -ENTITIES-    ~│~ List of entities                            ~│~\n" +
                         "~│~ -ENT_-*X*       ~│~ Spawn entity under ID X, with AI            ~│~\n" +
                         "~│~ -ENT_-*X*-_AI_-*Y*  ~│~ Spawn entity under ID X, Y = 1 - with AI    ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -NOCLIP-      ~│~ Enables/disables noclip                     ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -EGTRE-       ~│~ Give first aid kit                          ~│~\n" +
                         "~│~ -DHURF-       ~│~ Give adrenaline                             ~│~\n" +
                         "~│~ -KVISE-       ~│~ Give helmet                                 ~│~\n" +
                         "~│~ -YWJHC-       ~│~ Give medical kit                            ~│~\n" +
                         "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                         "~│~ -GKIFK-       ~│~ Give 999 HP                                 ~│~\n" +
                         "~│~ -KILL-        ~│~ Kill a player                               ~│~\n" +
                         "~│~ -LPFJY-       ~│~ Cause 99 damage                             ~│~\n" +
                         "~└─────────────┴─────────────────────────────────────────────┘~";
                }
                else
                    message += "You are -honest-! Or is that not true?";
            }
            else if (cheat == "OSTS")
            {
                show_date = false;
                string secret = "";
                string[] status = { "         ", "         ", "         ", "         ", "         ", "         " };
                status[SLIL.OstIndex] = "[PLAYING]";
                if (SLIL.OstIndex == 5)
                    secret = "~│~ -Soul Forge-  ~│~ " + status[5] + " ~│~\n";
                message = "\n" +
                     "~┌─────────────┬───────────┐~\n" +
                     "~│~ *Name*        ~│~ *Status*    ~│~\n" +
                     "~├─────────────┼───────────┤~\n" +
                     "~│~ -slil_ost_0-  ~│~ " + status[0] + " ~│~\n" +
                     "~│~ -slil_ost_1-  ~│~ " + status[1] + " ~│~\n" +
                     "~│~ -slil_ost_2-  ~│~ " + status[2] + " ~│~\n" +
                     "~│~ -slil_ost_3-  ~│~ " + status[3] + " ~│~\n" +
                     "~│~ -slil_ost_4-  ~│~ " + status[4] + " ~│~\n" +
                     secret +
                     "~└─────────────┴───────────┘~";
                message += "\nTo change the background music write OST_*OstIndex*";
            }
            else if (cheat.StartsWith("OST_"))
            {
                if (int.TryParse(cheat.Split('_')[1], out int x))
                {
                    if (ML.OutOfLimits(x, 4))
                    {
                        color = Color.Red;
                        message = "Incorrect value! X must be in the range from 0 to 4.";
                    }
                    else
                    {
                        message += $"Now the track slil_ost_{x} is playing.";
                        SLIL.PrevOst = x;
                        SLIL.ChangeOst(x);
                    }
                }
                else
                {
                    color = Color.Red;
                    message = "Incorrect data entered! X is not a number.";
                }
            }
            else if (cheat == "GUNS")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("\n~|───────────────────────|~");
                sb.AppendLine("~|~      *Weapon Name*      ~|~");
                sb.AppendLine("~|───────────────────────|~");
                int maxLength = 21;
                for (int i = 0; i < player.GUNS.Length; i++)
                {
                    string paddedName = player.GUNS[i].Name[1].PadRight(maxLength);
                    sb.AppendLine($"~|~ -{paddedName}- ~|~");
                }
                sb.AppendLine("~|───────────────────────|~");
                sb.AppendLine("To select a weapon, write GUN_*WeaponName*");
                show_date = false;
                message = sb.ToString();

            }
            else if (cheat == "ENEMIES")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("\n~|───────────────────────|~");
                sb.AppendLine("~|~      *Enemies List*      ~|~");
                sb.AppendLine("~|───────────────────────|~");
                int maxLength = 21;
                if (Entities.Count == 1)
                    sb.AppendLine("~|~      *No enemies.*      ~|~");
                for (int i = 0; i < Entities.Count; i++)
                {
                    if (Entities[i] is Creature)
                    {
                        Creature creature = Entities[i] as Creature;
                        if (!(creature is Enemy)) continue;
                        string dead = "";
                        if (creature.Dead) dead = "[Dead]";
                        string paddedName = $"Enemy #{i} {dead}".PadRight(maxLength);
                        sb.AppendLine($"~|~ -{paddedName}- ~|~");
                    }
                }
                sb.AppendLine("~|───────────────────────|~");
                sb.AppendLine("To select an enemy write Enemy_*EnemyIndex*");
                show_date = false;
                message = sb.ToString();
            }
            else if (cheat == "PLAYER")
            {
                string[] banList = { "Animations", "rand", "Guns", "GUNS", "Effects", "DisposableItems", "Name", "Angle", "IntX", "IntY", "VMove", "Frames" };
                PropertyInfo[] properties = typeof(Player).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                int maxPropNameLength = properties.Max(p => p.Name.Length);
                int maxValueLength = properties.Max(p => p.GetValue(player)?.ToString().Length ?? 0);
                int columnWidth = Math.Max(maxPropNameLength, maxValueLength);
                string line = new string('─', columnWidth + 2);
                StringBuilder table = new StringBuilder();
                table.AppendLine($"\n~┌{line}┬{line}┐~");
                table.AppendLine($"~│~ *{"Parameter".PadRight(columnWidth)}* ~│~ *{"Value".PadRight(columnWidth)}* ~│~");
                table.AppendLine($"~├{line}┼{line}┤~");
                foreach (PropertyInfo property in properties)
                {
                    if (banList.Contains(property.Name)) continue;
                    string propName = property.Name.PadRight(columnWidth);
                    object propValueObj = property.GetValue(player);
                    string propValue = "";
                    if (propValueObj is string[] names && names.Length > 0)
                        propValue = names[1];
                    else
                        propValue = propValueObj?.ToString() ?? "";
                    propValue = propValue.PadRight(columnWidth);
                    table.AppendLine($"~│~ -{propName}- ~│~ {propValue} ~│~");
                }
                table.AppendLine($"~└{line}┴{line}┘~");
                show_date = false;
                message = table.ToString();
            }
            else if (cheat.StartsWith("ENEMY_"))
            {
                Enemy selected = null;
                try
                {
                    int index = Convert.ToInt32(cheat.Split('_')[1]);
                    if (Entities[index] is Enemy enemy && Entities.Contains(Entities[index]))
                        selected = enemy;
                }
                catch { }
                if (selected == null)
                {
                    CheatIndex = History.Count - 1;
                    color = Color.Red;
                    message += "There is no enemy under this index.";
                }
                else
                {
                    string[] banList = { "Animations", "Rand", "ImpassibleCells", "MapWidth" };
                    PropertyInfo[] properties = typeof(Enemy).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    int maxPropNameLength = properties.Max(p => p.Name.Length);
                    int maxValueLength = properties.Max(p => p.GetValue(selected)?.ToString().Length ?? 0);
                    int columnWidth = Math.Max(maxPropNameLength, maxValueLength);
                    string line = new string('─', columnWidth + 2);
                    StringBuilder table = new StringBuilder();
                    table.AppendLine($"\n~┌{line}┬{line}┐~");
                    table.AppendLine($"~│~ *{"Parameter".PadRight(columnWidth)}* ~│~ *{"Value".PadRight(columnWidth)}* ~│~");
                    table.AppendLine($"~├{line}┼{line}┤~");
                    table.AppendLine($"~│~ -{"Name".PadRight(columnWidth)}- ~│~ {selected.GetType().Name.PadRight(columnWidth)} ~│~");
                    foreach (PropertyInfo property in properties)
                    {
                        if (banList.Contains(property.Name)) continue;
                        string propName = property.Name.PadRight(columnWidth);
                        object propValueObj = property.GetValue(selected);
                        string propValue = "";
                        if (propValueObj is string[] names && names.Length > 0)
                            propValue = names[1];
                        else
                            propValue = propValueObj?.ToString() ?? "";
                        propValue = propValue.PadRight(columnWidth);
                        table.AppendLine($"~│~ -{propName}- ~│~ {propValue} ~│~");
                    }
                    table.AppendLine($"~└{line}┴{line}┘~");
                    show_date = false;
                    message = table.ToString();
                }
            }
            else if (cheat.StartsWith("GUN_"))
            {
                string name = cheat.Split('_')[1];
                Gun selected = null;
                foreach (Gun gun in player.GUNS)
                {
                    if (gun.Name[1].ToLower() == name.ToLower())
                    {
                        selected = gun;
                        break;
                    }
                }
                if (selected == null)
                {
                    CheatIndex = History.Count - 1;
                    color = Color.Red;
                    message += "This weapon is not on the list.";
                }
                else
                {
                    PropertyInfo[] properties = typeof(Gun).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    int maxPropNameLength = properties.Max(p => p.Name.Length);
                    int maxValueLength = properties.Max(p => p.GetValue(selected)?.ToString().Length ?? 0);
                    int columnWidth = Math.Max(maxPropNameLength, maxValueLength);
                    string line = new string('─', columnWidth + 2);
                    StringBuilder table = new StringBuilder();
                    table.AppendLine($"\n~┌{line}┬{line}┐~");
                    table.AppendLine($"~│~ *{"Parameter".PadRight(columnWidth)}* ~│~ *{"Value".PadRight(columnWidth)}* ~│~");
                    table.AppendLine($"~├{line}┼{line}┤~");
                    foreach (PropertyInfo property in properties)
                    {
                        string propName = property.Name.PadRight(columnWidth);
                        object propValueObj = property.GetValue(selected);
                        string propValue = "";
                        if (propValueObj is string[] names && names.Length > 0)
                            propValue = names[1];
                        else
                            propValue = propValueObj?.ToString() ?? "";
                        propValue = propValue.PadRight(columnWidth);
                        table.AppendLine($"~│~ -{propName}- ~│~ {propValue} ~│~");
                    }
                    table.AppendLine($"~└{line}┴{line}┘~");
                    show_date = false;
                    message = table.ToString();
                }
            }
            else if (cheat == "IMHONEST")
            {
                if (!ImHonest)
                {
                    ImHonest = true;
                    message = "You're above the rest for choosing the -honest- path. Respect for keeping the game real!";
                }
                else
                    message = "This action cannot be undone! You are -honest- aren't you?)";
            }
            else if (cheat == "CLS")
            {
                show_date = false;
                console.Text = null;
                message = "SLIL console *v1.6*\nType \"-HELP-\" for a list of commands...";
                console.Refresh();
            }
            else if (cheat == "SLC")
            {
                show_message = false;
                History.Clear();
            }
            else if (cheat == "SAY GEX") message += "GAY SEX";
            else if (cheat.StartsWith("SAY "))
            {
                string[] say = cheat.Split(' ');
                for (int i = 1; i < say.Length; i++)
                    message += say[i] + " ";
            }
            else if (cheat == "FPS")
            {
                parent.ShowFPS = !parent.ShowFPS;
                Program.iniReader.SetKey("SLIL", "show_fps", parent.ShowFPS);
                if (parent.ShowFPS)
                    message += "FPS display enabled.";
                else
                    message += "FPS display disabled.";
            }
            else if (cheat == "MINIMAP")
            {
                parent.ShowMiniMap = !parent.ShowMiniMap;
                Program.iniReader.SetKey("SLIL", "show_minimap", parent.ShowMiniMap);
                if (parent.ShowMiniMap)
                    message += "Minimap enabled.";
                else
                    message += "Minimap disabled.";
            }
            else if (cheat.StartsWith("MVOL_"))
            {
                if (float.TryParse(cheat.Split('_')[1].Replace('.', ','), out float x))
                {
                    if (ML.WithinOne(x))
                    {
                        message += $"Current music volume is now {x}. *Default: 0,4*";
                        SLIL.MusicVolume = x;
                        SLIL.SetVolume();
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Incorrect value! X must be in the range from 0 to 1.";
                    }
                }
                else
                {
                    color = Color.Red;
                    message = "Incorrect data entered! X is not a number.";
                }
            }
            else if (cheat.StartsWith("EVOL_"))
            {
                if (float.TryParse(cheat.Split('_')[1].Replace('.', ','), out float x))
                {
                    if (ML.WithinOne(x))
                    {
                        message += $"Current effects volume is now {x}. *Default: 0,4*";
                        SLIL.EffectsVolume = x;
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Incorrect value! X must be in the range from 0 to 1.";
                    }
                }
                else
                {
                    color = Color.Red;
                    message = "Incorrect data entered! X is not a number.";
                }
            }
            else if (cheat.StartsWith("VOL_"))
            {
                if (float.TryParse(cheat.Split('_')[1].Replace('.', ','), out float x))
                {
                    if (ML.WithinOne(x))
                    {
                        message += $"Current volume is now {x}. *Default: 0,4*";
                        SLIL.Volume = x;
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Incorrect value! X must be in the range from 0 to 1.";
                    }
                }
                else
                {
                    color = Color.Red;
                    message = "Incorrect data entered! X is not a number.";
                }
            }
            else if (cheat.StartsWith("SCOPE_"))
            {
                if (int.TryParse(cheat.Split('_')[1], out int x))
                {
                    if (ML.OutOfLimits(x, 4))
                    {
                        color = Color.Red;
                        message = "Incorrect value! X must be in the range from 0 to 4.";
                    }
                    else
                    {
                        message += $"Current crosshair is now {x}. *Default: 0*";
                        SLIL.ScopeType = x;
                        Program.iniReader.SetKey("SLIL", "scope_type", x);
                    }
                }
                else
                {
                    color = Color.Red;
                    message = "Incorrect data entered! X is not a number.";
                }
            }
            else if (cheat.StartsWith("SCOPECOL_"))
            {
                if (int.TryParse(cheat.Split('_')[1], out int x))
                {
                    if (ML.OutOfLimits(x, 8))
                    {
                        color = Color.Red;
                        message = "Incorrect value! X must be in the range from 0 to 8.";
                    }
                    else
                    {
                        message += $"Current crosshair color is now {x}. *Default: 0*";
                        SLIL.ScopeColor = x;
                        Program.iniReader.SetKey("SLIL", "scope_color", x);
                    }
                }
                else
                {
                    color = Color.Red;
                    message = "Incorrect data entered! X is not a number.";
                }
            }
            else if (cheat.StartsWith("LOOK_"))
            {
                if (double.TryParse(cheat.Split('_')[1].Replace('.', ','), out double x))
                {
                    if (ML.OutOfLimits(x, 2.5, 10))
                    {
                        color = Color.Red;
                        message = "Incorrect range specified! Instead of X, enter a number between 2,5 and 10.";
                    }
                    else
                    {
                        message += $"Mouse sensitivity is now {x}. *Default: 2,75*";
                        SLIL.LookSpeed = x;
                        Program.iniReader.SetKey("SLIL", "look_speed", x);
                    }
                }
                else
                {
                    color = Color.Red;
                    message = "Incorrect data entered! X is not a number.";
                }
            }
            else if (cheat.StartsWith("COLOR_"))
            {
                if (int.TryParse(cheat.Split('_')[1], out int x))
                {
                    if (ML.OutOfLimits(x, 10))
                    {
                        color = Color.Red;
                        message = "Incorrect range specified! Instead of X, enter a number between 0 and 10.";
                    }
                    else
                    {
                        ColorIndex = x;
                        color = ForeColors[ColorIndex];
                        show_date = false;
                        console.Text = null;
                        message = "SLIL console *v1.6*\nType \"-HELP-\" for a list of commands...";
                        console.Refresh();
                    }
                }
                else
                {
                    color = Color.Red;
                    message = "Incorrect data entered! X is not a number.";
                }
            }
            else if (cheat == "DUMMY")
            {
                if (parent.SpawnEntity(29, true))
                    message = $"Dummy successfully spawned";
                else
                {
                    color = Color.Red;
                    message = $"There was an error while spawning a dummy. Most likely a wall got in the way.";
                }
            }
            else if (!ImHonest)
            {
                if (cheat == "EFFECTS")
                {
                    show_date = false;
                    message = "\n" +
                         "~┌─────┬──────────────────┬─────────────────────────────────────┐~\n" +
                         "~│~ *ID*  ~│~ *Effect*           ~│~ *Description*                         ~│~\n" +
                         "~├─────┼──────────────────┼─────────────────────────────────────┤~\n";
                    for (int i = 0; i < Effects.Length; i++)
                    {
                        string id = Effects[i].ID.ToString().PadRight(4);
                        string name = Effects[i].Name.PadRight(17);
                        string description = Effects[i].Description.PadRight(36);
                        message += $"~│~ *{id}*~│~ -{name}-~│~ {description}~│~\n";
                    }
                    message += "~└─────┴──────────────────┴─────────────────────────────────────┘~";
                }
                else if (cheat == "EFCLEAR")
                {
                    player.StopEffects();
                    message = "Effects have been cleared";
                }
                else if (cheat == "EFALLGV")
                {
                    for (int i = 0; i < Effects.Length; i++)
                    {
                        if (!Effects[i].CanIssuedByConsole) continue;
                        if (!player.EffectCheck(i) && i != 4 && i != 8)
                            player.GiveEffect(i, true);
                    }
                    message = $"All effects have been issued";
                }
                else if (cheat.StartsWith("EFGIVE_"))
                {
                    if (int.TryParse(cheat.Split('_')[1], out int x))
                    {
                        if (ML.OutOfLimits(x, Effects.Length))
                        {
                            color = Color.Red;
                            message = $"There is no effect under ID {x}";
                        }
                        else
                        {
                            if (player.EffectCheck(x))
                            {
                                color = Color.Red;
                                message = $"You already have an effect under ID {x}";
                            }
                            else
                            {
                                if (!Effects[x].CanIssuedByConsole)
                                {
                                    color = Color.Red;
                                    message = $"It is impossible to issue this effect with the command";
                                }
                                else
                                {
                                    player.GiveEffect(x, true);
                                    message = $"The effect under ID {x} is issued";
                                }
                            }
                        }
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Incorrect data entered! X is not a number.";
                    }
                }
                else if (cheat.StartsWith("EFGINF_"))
                {
                    if (int.TryParse(cheat.Split('_')[1], out int x))
                    {
                        if (ML.OutOfLimits(x, Effects.Length))
                        {
                            color = Color.Red;
                            message = $"There is no effect under ID {x}";
                        }
                        else
                        {
                            if (player.EffectCheck(x))
                            {
                                color = Color.Red;
                                message = $"You already have an effect under ID {x}";
                            }
                            else
                            {
                                if (!Effects[x].CanIssuedByConsole)
                                {
                                    color = Color.Red;
                                    message = $"It is impossible to issue this effect with the command";
                                }
                                else
                                {
                                    player.GiveEffect(x, true, 0, true);
                                    message = $"The effect under ID {x} is issued";
                                }
                            }
                        }
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Incorrect data entered! X is not a number.";
                    }
                }
                else if (cheat.StartsWith("EFG_") && cheat.Contains("_TM_"))
                {
                    try
                    {
                        int x = Convert.ToInt32(cheat.Split('_')[1]);
                        int y = Convert.ToInt32(cheat.Split('_')[3]);
                        if (ML.OutOfLimits(x, Effects.Length))
                        {
                            color = Color.Red;
                            message = $"There is no effect under ID {x}";
                        }
                        else
                        {
                            if (ML.OutOfLimits(y, 5, 999))
                            {
                                color = Color.Red;
                                message = "Incorrect value! Y must be in the range from 5 to 999.";
                            }
                            else
                            {
                                if (!Effects[x].CanIssuedByConsole)
                                {
                                    color = Color.Red;
                                    message = $"It is impossible to issue this effect with the command";
                                }
                                else
                                {
                                    player.GiveEffect(x, false, y);
                                    message = $"Effect at ID {x} was issued for {y} seconds";
                                }
                            }
                        }
                    }
                    catch
                    {
                        color = Color.Red;
                        message = "Incorrect data entered! X or Y is not a number.";
                    }
                }
                else if (cheat == "CAT")
                {
                    if (!(player.PET is SillyCat))
                    {
                        message += "Pet \"Silly cat\" has been issued.";
                        player.ChangeMoney(parent.GetPetCost(0));
                        parent.AddPet(0);
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You already have \"Silly cat\".";
                    }
                }
                else if (cheat == "GNOME")
                {
                    if (!(player.PET is GreenGnome))
                    {
                        message += "Pet \"Wizard Gnome\" has been issued.";
                        player.ChangeMoney(parent.GetPetCost(1));
                        parent.AddPet(1);
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You already have \"Wizard Gnome\".";
                    }
                }
                else if (cheat == "ENERGY")
                {
                    if (!(player.PET is EnergyDrink))
                    {
                        message += "Pet \"Energy Drink\" has been issued.";
                        player.ChangeMoney(parent.GetPetCost(2));
                        parent.AddPet(2);
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You already have \"Energy Drink\".";
                    }
                }
                else if (cheat == "ILOVEFURRY")
                {
                    if (!(player.PET is Pyro))
                    {
                        message += "Pet \"Podseratel\" has been issued.";
                        player.ChangeMoney(parent.GetPetCost(3));
                        parent.AddPet(3);
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You already have \"Podseratel\".";
                    }
                }
                else if (cheat.StartsWith("CHSCH_"))
                {
                    if (double.TryParse(cheat.Split('_')[1].Replace('.', ','), out double x))
                    {
                        if (ML.WithinOne(x))
                        {
                            message += $"Set chance of curse healing to {x * 100:0.##}% *Default: 8%*";
                            player.CurseCureChance = x;
                        }
                        else
                        {
                            color = Color.Red;
                            message = "Incorrect range specified! Instead of X, enter a number between 0 and 1.";
                        }
                    }
                }
                else if (cheat.StartsWith("DKSCH_"))
                {
                    if (double.TryParse(cheat.Split('_')[1].Replace('.', ','), out double x))
                    {
                        if (ML.WithinOne(x))
                        {
                            message += $"Set chance of damn kick to {x * 100:0.##}% *Default: 4%*";
                            player.CurseKickChance = x;
                        }
                        else
                        {
                            color = Color.Red;
                            message = "Incorrect range specified! Instead of X, enter a number between 0 and 1.";
                        }
                    }
                }
                else if (cheat.StartsWith("MONEY_"))
                {
                    if (int.TryParse(cheat.Split('_')[1], out int x))
                    {
                        message += $"The amount of money has been changed to {x}";
                        player.ChangeMoney(x);
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Incorrect data entered! X is not a number.";
                    }
                }
                else if (cheat.StartsWith("STAMIN_"))
                {
                    if (int.TryParse(cheat.Split('_')[1], out int x))
                    {
                        if (ML.OutOfLimits(x, 100, 5000))
                        {
                            color = Color.Red;
                            message = "Incorrect value! X must be in the range from 100 to 5000.";
                        }
                        else
                        {
                            message += $"Player stamina is now {x}. *Default: 650*";
                            player.MaxStamine = x;
                            player.Stamine = x;
                        }
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Incorrect data entered! X is not a number.";
                    }
                }
                else if (cheat.StartsWith("SPEED_"))
                {
                    if (double.TryParse(cheat.Split('_')[1], out double x))
                    {
                        if (ML.OutOfLimits(x, 0.1, 20))
                        {
                            color = Color.Red;
                            message = "Incorrect value! X must be in the range from 0,1 to 20.";
                        }
                        else
                        {
                            message += $"Player speed is now {x}. *Default: 1,8*";
                            player.MaxMoveSpeed = x;
                            player.MaxStrafeSpeed = x / 2;
                        }
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Incorrect data entered! X is not a number.";
                    }
                }
                else if (cheat == "ENTITIES")
                {
                    show_date = false;
                    message = "\n" +
                         "~┌─────┬──────────────────┐~\n" +
                         "~│~ *ID*  ~│~ *Entity*           ~│~\n" +
                         "~├─────┼──────────────────┤~\n";
                    string[] entities =
                    {
                                 "PlayerDeadBody",
                                 "Zombie",
                                 "Dog",
                                 "Ogr",
                                 "Bat",
                                 "Box",
                                 "Barrel",
                                 "ShopDoor",
                                 "ShopMan" ,
                                 "Teleport",
                                 "HittingTheWall",
                                 "RpgRocket",
                                 "RpgExplosion",
                                 "SillyCat",
                                 "GreenGnome",
                                 "EnergyDrink",
                                 "Pyro",
                                 "Bike",
                                 "Vine",
                                 "Lamp",
                                 "BackroomsTeleport",
                                 "VoidTeleport",
                                 "VoidStalker",
                                 "Stalker",
                                 "Shooter",
                                 "LostSoul",
                                 "SoulExplosion",
                                 "ExplodingBarrel",
                                 "BarrelExplosion",
                                 "Dummy",
                            };
                    for (int i = 0; i < entities.Length; i++)
                    {
                        string id = i.ToString().PadRight(4);
                        string name = entities[i].PadRight(17);
                        message += $"~│~ *{id}*~│~ -{name}-~│~\n";
                    }
                    message += "~└─────┴──────────────────┘~\n" +
                        "-ENT_-*X*-_AI_-*Y* Spawn entity under ID X, Y = 1 - with AI\n" +
                        "-ENT_-*X* Spawn entity under ID X, with AI\n";
                }
                else if (cheat.StartsWith("ENT_"))
                {
                    if (cheat.Contains("_AI_"))
                    {
                        try
                        {
                            int x = Convert.ToInt32(cheat.Split('_')[1]);
                            int y = Convert.ToInt32(cheat.Split('_')[3]);
                            if (ML.OutOfLimits(x, 30))
                            {
                                color = Color.Red;
                                message = $"There is no entity under ID {x}";
                            }
                            else
                            {
                                if (ML.WithinOne(y))
                                {
                                    if (parent.SpawnEntity(x, y == 1))
                                    {
                                        if (y == 0) message = $"Creature with ID {x} successfully spawned with AI disabled";
                                        else message = $"Creature with ID {x} successfully spawned with AI enabled";
                                    }
                                    else
                                    {
                                        color = Color.Red;
                                        message = $"There was an error while spawning the entity. Most likely a wall got in the way.";
                                    }
                                }
                                else
                                {
                                    color = Color.Red;
                                    message = "Incorrect value! Y must be 0 or 1";
                                }
                            }
                        }
                        catch
                        {
                            color = Color.Red;
                            message = "Incorrect data entered! X or Y is not a number.";
                        }
                    }
                    else
                    {
                        if (int.TryParse(cheat.Split('_')[1], out int x))
                        {
                            if (ML.OutOfLimits(x, 30))
                            {
                                color = Color.Red;
                                message = $"There is no entity under ID {x}";
                            }
                            else
                            {
                                if (parent.SpawnEntity(x, true))
                                    message = $"Creature with ID {x} successfully spawned with AI enabled";
                                else
                                {
                                    color = Color.Red;
                                    message = $"There was an error while spawning the entity. Most likely a wall got in the way.";
                                }
                            }
                        }
                        else
                        {
                            color = Color.Red;
                            message = "Incorrect data entered! X or Y is not a number.";
                        }
                    }
                }
                else if (cheat == "NOCLIP")
                {
                    if (parent.OnOffNoClip()) message = "noclip enabled";
                    else message = "noclip disabled";
                }
                else if (cheat == "GOD")
                {
                    if (player.EffectCheck(9))
                    {
                        player.StopEffect(9);
                        message = "god mod enabled";
                    }
                    else
                    {
                        player.GiveEffect(9, true);
                        message = "god mod disabled";
                    }
                }
                else if (cheat == "KILL")
                {
                    show_message = false;
                    parent.KillFromConsole();
                }
                else if (cheat == "BEFWK")
                {
                    bool all_ok = false;
                    for (int i = 0; i < player.GUNS.Length; i++)
                    {
                        if (player.GUNS[i].AddToShop)
                        {
                            player.GUNS[i].AmmoInStock = player.GUNS[i].MaxAmmo;
                            player.GUNS[i].HasIt = true;
                            if (!player.Guns.Contains(player.GUNS[i]))
                            {
                                player.Guns.Add(player.GUNS[i]);
                                all_ok = true;
                            }
                        }
                    }
                    if (all_ok)
                        message += "All weapons have been issued.";
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You already have all the weapons.";
                    }
                }
                else if (cheat == "IDDQD")
                {
                    bool can_do_it = false;
                    for (int i = 0; i < player.Guns.Count; i++)
                    {
                        if (player.Guns[i].CanUpdate())
                            can_do_it = true;
                    }
                    if (can_do_it)
                    {
                        for (int i = 0; i < player.Guns.Count; i++)
                            player.Guns[i].LevelUpdate();
                        player.LevelUpdated = true;
                        message += "All weapon levels increased by one.";
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You have already pumped all your weapons to the maximum.";
                    }
                }
                else if (cheat == "FYTLG")
                {
                    for (int i = 0; i < player.GUNS.Length; i++)
                    {
                        if (!(player.GUNS[i] is Item) && player.GUNS[i].HasIt)
                            player.GUNS[i].AmmoInStock = player.GUNS[i].MaxAmmo;
                    }
                    message += "Maximum ammunition provided.";
                }
                else if (cheat == "SOTLG")
                {
                    player.Money = 9999;
                    message += "Maximum money granted.";
                }
                else if (cheat == "YHRII")
                {
                    if (!player.GUNS[7].HasIt)
                    {
                        player.GUNS[7].HasIt = true;
                        player.GUNS[7].AmmoInStock = player.GUNS[7].MaxAmmo;
                        if (!player.Guns.Contains(player.GUNS[7]))
                            player.Guns.Add(player.GUNS[7]);
                        message += "\"Fingershot\" issued.";
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You already have \"Fingershot\".";
                    }
                }
                else if (cheat == "BIGGUY")
                {
                    if (!player.GUNS[8].HasIt)
                    {
                        player.AddWeapon(player.GUNS[8]);
                        message += "\"The Smallest Pistol in the World\" has been issued.";
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You already have \"The Smallest Pistol in the World\"";
                    }
                }
                else if (cheat == "IMGNOME")
                {
                    if (!player.GUNS[9].HasIt)
                    {
                        player.AddWeapon(player.GUNS[9]);
                        message += "\"Wizard Gnome\" has been issued.";
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You already have \"Wizard Gnome\"";
                    }
                }
                else if (cheat == "ILLKLURDOG")
                {
                    if (!player.GUNS[16].HasIt)
                    {
                        player.AddWeapon(player.GUNS[16]);
                        message += "\"Petition\" has been issued.";
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You already have \"Petition\"";
                    }
                }
                else if (cheat == "COMEGETSOME")
                {
                    if (!player.GUNS[18].HasIt)
                    {
                        player.AddWeapon(player.GUNS[18], false);
                        message += "\"Minigun\" has been issued.";
                    }
                    else
                    {
                        color = Color.Red;
                        message = "Code not applied! You already have \"Minigun\"";
                    }
                }
                else if (cheat == "GKIFK")
                {
                    player.HP = 999;
                    message += "Maximum HP granted.";
                }
                else if (cheat == "EGTRE")
                {
                    GetItem(0);
                    message += "First aid kits issued.";
                }
                else if (cheat == "DHURF")
                {
                    GetItem(1);
                    message += "Adrenaline issued.";
                }
                else if (cheat == "KVISE")
                {
                    GetItem(2);
                    message += "Helmet issued.";
                }
                else if (cheat == "YWJHC")
                {
                    GetItem(3);
                    message += "Medical kit issued.";
                }
                else if (cheat == "LPFJY")
                {
                    player.HP = 1;
                    message += "Health reduced by 99.";
                }
                else
                {
                    CheatIndex = History.Count - 1;
                    color = Color.Red;
                    message = $"Unknown command: {cheat}";
                }
            }
            else
            {
                CheatIndex = History.Count - 1;
                color = Color.Red;
                message = $"Unknown command: {cheat}";
            }
            if (show_date) time = $"\n-<{DateTime.Now:HH:mm}>- ";
            if (show_message) ConsoleAppendText($"\n{time}{message}", color);
            if (color != Color.Red)
            {
                History.Add(cheat);
                CheatIndex = History.Count - 1;
            }
            ConsoleAppendText("\n\nEnter the command: ", ForeColors[ColorIndex]);
            console.ScrollToCaret();
        }

        internal void ClearCommand()
        {
            ConsoleDeleteText(Command.Length);
            Command = "";
        }

        private void ConsoleDeleteText(int count)
        {
            if (count == 0) return;
            int length = console.Text.Length;
            console.Select(length - count, count);
            console.ReadOnly = false;
            console.SelectedText = "";
            console.ReadOnly = true;
            console.Select(length - count, 0);
        }

        private void ConsoleAppendText(string text, Color color)
        {
            string pattern = string.Join("|", ColorMap.Keys.Select(k => $@"(\{k}.*?\{k})"));
            string[] parts = Regex.Split(text, pattern);
            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;
                var colorPair = ColorMap.FirstOrDefault(pair => part.StartsWith(pair.Key) && part.EndsWith(pair.Key));
                if (colorPair.Key != null)
                {
                    string word = part.Trim(colorPair.Key.ToCharArray());
                    ConsoleAppendColoredText(word, colorPair.Value);
                }
                else
                    ConsoleAppendColoredText(part, color);
            }
            console.SelectionStart = console.Text.Length;
            console.ScrollToCaret();
        }

        private void ConsoleAppendColoredText(string text, Color color)
        {
            console.SelectionStart = console.Text.Length;
            console.SelectionLength = 0;
            console.SelectionColor = color;
            console.AppendText(text);
        }

        private void Console_panel_VisibleChanged(object sender, EventArgs e)
        {
            BringToFront();
            if (player != null)
                player.Look = 0;
            CheatIndex = History.Count - 1;
        }

        private void Console_TextChanged(object sender, EventArgs e)
        {
            if (console.Text.Length == console.MaxLength)
            {
                console.Clear();
                ConsoleAppendText("SLIL console *v1.6*\nType \"-HELP-\" for a list of commands...", ForeColors[ColorIndex]);
                ConsoleAppendText("*The console was cleared due to a buffer overflow*", ForeColors[ColorIndex]);
                ConsoleAppendText("\n\nEnter the command: ", ForeColors[ColorIndex]);
                console.Refresh();
            }
        }

        internal void Log(string message, bool newline, bool showTime, Color color)
        {
            string nline = "", time = $"-<{DateTime.Now:HH:mm}>- ";
            if (newline)
                nline = "\n";
            if (!showTime)
                time = "";
            ConsoleAppendText($"{nline}{time}{message}", color);
        }

        private void Console_LinkClicked(object sender, LinkClickedEventArgs e) => Process.Start(new ProcessStartInfo(e.LinkText) { UseShellExecute = true });

        private void Console_MouseClick(object sender, MouseEventArgs e)
        {
            int charIndex = console.GetCharIndexFromPosition(e.Location);
            if (charIndex < 0 || charIndex >= console.TextLength) return;
            int lineIndex = console.GetLineFromCharIndex(charIndex);
            if (lineIndex < 0 || lineIndex >= console.Lines.Length) return;
            string line = console.Lines[lineIndex];
            int charPositionInLine = charIndex - console.GetFirstCharIndexFromLine(lineIndex);
            const string pattern = @"screenshots\\screenshot_\d{4}_\d{2}_\d{2}__\d{2}_\d{2}_\d{2}\.png";
            Match match = Regex.Match(line, pattern);
            if (match.Success)
            {
                string filePath = match.Value;
                int startIndex = match.Index;
                int endIndex = startIndex + filePath.Length;
                if (charPositionInLine >= startIndex && charPositionInLine < endIndex)
                {
                    if (File.Exists(filePath))
                    {
                        try { Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true }); }
                        catch (Exception ex) { Log($"Error opening file: {ex.Message}", true, true, Color.Red); }
                    }
                    else
                        Log($"File not found: {filePath}", true, true, Color.Red);
                }
            }
        }

        private void Console_Load(object sender, EventArgs e) => console.Refresh();
    }
}