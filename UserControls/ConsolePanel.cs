using IniReader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SLIL.Classes;

namespace SLIL.UserControls
{
    public partial class ConsolePanel : UserControl
    {
        private static bool ImHonest = false;
        private int cheat_index = 0, color_index = 0;
        private readonly Effect[] effects = { new Regeneration(), new Adrenaline(), new Protection(), new Fatigue() };
        private readonly List<string> previous_cheat = new List<string>();
        public List<Entity> Entities;
        public Player player;
        public string command = "";
        private readonly Dictionary<string, Color> colorMap = new Dictionary<string, Color>
        {
            { "-", Color.Yellow },
            { "*", Color.Tomato },
            { "~", Color.Cyan },
            { "<", Color.White }
        };
        private readonly Color[] foreColors = 
        {
            Color.Lime, Color.White, Color.Magenta, 
            Color.Teal,Color.DeepSkyBlue, Color.SlateGray, 
            Color.Violet, Color.SandyBrown, Color.SpringGreen, 
            Color.Aquamarine, Color.Sienna
        };

        public ConsolePanel()
        {
            InitializeComponent();
        }

        private void GetItem(int index) => player.DisposableItems[index].AddItem();

        private void Console_KeyDown(object sender, KeyEventArgs e) => e.SuppressKeyPress = true;

        private void Console_KeyUp(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            e.Handled = true;
            SLIL parent = (Parent.FindForm() as SLIL);
            if (e.KeyCode == Keys.Enter)
            {
                Color color = foreColors[color_index];
                if (command.Length > 0)
                {
                    bool show_date = true, show_message = true;
                    string message = null, time = null;
                    string cheat = command.ToUpper().Trim(' ').Replace("`", null);
                    command = "";
                    if (cheat == "HELP")
                    {
                        show_date = false;
                        message = "\n" +
                             "~┌─────────────┬─────────────────────────────────────────────┐~\n" +
                             "~│~ *Command*     ~│~ *Description*                                 ~│~\n" +
                             "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                             "~│~ -IMHONEST-    ~│~ Disable cheats                              ~│~\n" +
                             "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                             "~│~ -FPS-         ~│~ Show/hide FPS                               ~│~\n" +
                             "~│~ -MINIMAP-     ~│~ Show/hide Minimap                           ~│~\n" +
                             "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                             "~│~ -CLS-         ~│~ Clearing the console                        ~│~\n" +
                             "~│~ -SLC-         ~│~ Clear console history                       ~│~\n" +
                             "~│~ -COLOR_-*X*     ~│~ Change console font color                   ~│~\n" +
                             "~│~ -VOL_-*X*       ~│~ Change volume of sounds to X                ~│~\n" +
                             "~│~ -SCOPE_-*X*     ~│~ Replace current sight                       ~│~\n" +
                             "~│~ -SCOPECOL_-*X*  ~│~ Change sight color                          ~│~\n" +
                             "~│~ -LOOK_-*X*      ~│~ Change mouse sensitivity                    ~│~\n" +
                             "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                             "~│~ -PLAYER-      ~│~ View player information                     ~│~\n" +
                             "~│~ -GUNS-        ~│~ Viewing weapon parameters                   ~│~\n" +
                             "~│~ -ENEMYS-      ~│~ View list of enemies                        ~│~\n" +
                             "~│~ -OSTS-        ~│~ View a list of game background music        ~│~\n" +
                             "~│~ -CHEATS-      ~│~ View list of cheats                         ~│~\n" +
                             "~└─────────────┴─────────────────────────────────────────────┘~";

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
                                 "~│~ -CCHANC_-*X*    ~│~ Set the probability of cursed treatment     ~│~\n" +
                                 "~│~ -MONEY_-*X*     ~│~ Change the amount of money to X             ~│~\n" +
                                 "~│~ -SOTLG-       ~│~ Maximum amount of money                     ~│~\n" +
                                 "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                                 "~│~ -STAMIN_-*X*    ~│~ Changing player maximum stamina             ~│~\n" +
                                 "~│~ -SPEED_-*X*     ~│~ Changing player movement speed              ~│~\n" +
                                 "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                                 "~│~ -BIGGUY-      ~│~ Give out \"The Smallest Pistol in the World\" ~│~\n" +
                                 "~│~ -YHRII-       ~│~ Issue \"Fingershot\"                          ~│~\n" +
                                 "~│~ -IMGNOME-     ~│~ Issue \"Wizard Gnome\"                        ~│~\n" +
                                 "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                                 "~│~ -CAT-         ~│~ Issue a pet: \"Silly cat\"                    ~│~\n" +
                                 "~│~ -GNOME-       ~│~ Issue a pet: \"Wizard Gnome\"                 ~│~\n" +
                                 "~│~ -ENERGY-      ~│~ Issue a pet: \"Energy Drink\"                 ~│~\n" +
                                 "~│~ -ILOVEFURRY-  ~│~ Issue a pet: \"Podseratel\"                   ~│~\n" +
                                 "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                                 "~│~ -BEFWK-       ~│~ Issue out all weapons                       ~│~\n" +
                                 "~│~ -FYTLG-       ~│~ Maximum amount of ammunition                ~│~\n" +
                                 "~│~ -IDDQD-       ~│~ Upgrade all weapons by one level            ~│~\n" +
                                 "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                                 "~│~ -EFFECTS-     ~│~ List of effects                             ~│~\n" +
                                 "~│~ -EFALLGV-     ~│~ Give all effects                            ~│~\n" +
                                 "~│~ -EFCLEAR-     ~│~ Cleaning up effects                         ~│~\n" +
                                 "~│~ -EFGIVE_-*X*    ~│~ Issue effect under X id                     ~│~\n" +
                                 "~│~ -EFG_-*X*-_TM_-*Y*  ~│~ Issue effect under X id for Y seconds       ~│~\n" +
                                 "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                                 "~│~ -ENTITIES-    ~│~ List of entities                            ~│~\n" +
                                 "~│~ -ENT_-*X*       ~│~ Spawn entity under ID X, with AI          ~│~\n" +
                                 "~│~ -ENT_-*X*-_AI_-*Y*  ~│~ Spawn entity under ID X, Y = 1 - with AI    ~│~\n" +
                                 "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                                 "~│~ -NOCLIP-      ~│~ Enables/disables noclip                     ~│~\n" +
                                 "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                                 "~│~ -EGTRE-       ~│~ Issue first aid kits                        ~│~\n" +
                                 "~│~ -DHURF-       ~│~ Issue adrenaline                            ~│~\n" +
                                 "~│~ -KVISE-       ~│~ Issue helmet                                ~│~\n" +
                                 "~├─────────────┼─────────────────────────────────────────────┤~\n" +
                                 "~│~ -GKIFK-       ~│~ Issue 999 HP                                ~│~\n" +
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
                        status[SLIL.ost_index] = "[PLAYING]";
                        if (SLIL.ost_index == 5)
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
                        try
                        {
                            int x = Convert.ToInt32(cheat.Split('_')[1]);
                            if (x > -1 && x < 5)
                            {
                                message += $"Now the track slil_ost_{x} is playing.";
                                SLIL.prev_ost = x;
                                SLIL.ChangeOst(x);
                            }
                            else
                            {
                                color = Color.Red;
                                message = "Incorrect value! X must be in the range from 0 to 4.";
                            }
                        }
                        catch
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
                    else if (cheat == "ENEMYS")
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("\n~|───────────────────────|~");
                        sb.AppendLine("~|~      *Enemys List*      ~|~");
                        sb.AppendLine("~|───────────────────────|~");
                        int maxLength = 21;
                        if (Entities.Count == 1)
                            sb.AppendLine("~|~      *No enemies.*      ~|~");
                        for (int i = 0; i < Entities.Count; i++)
                        {
                            if (Entities[i] is Creature)
                            {
                                Creature creature = Entities[i] as Creature;
                                if (!(creature is Enemy))
                                    continue;
                                string dead = "";
                                if (creature.DEAD)
                                    dead = "[DEAD]";
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
                        PropertyInfo[] properties = typeof(Player).GetProperties();
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
                            if (property.Name == "Guns" || property.Name == "FirstAidKits")
                                continue;
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
                            cheat_index = previous_cheat.Count - 1;
                            color = Color.Red;
                            message += "There is no enemy under this index.";
                        }
                        else
                        {
                            PropertyInfo[] properties = typeof(Enemy).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
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
                                if (property.Name == "rand" || property.Name == "MAP_WIDTH")
                                    continue;
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
                            cheat_index = previous_cheat.Count - 1;
                            color = Color.Red;
                            message += "This weapon is not on the list.";
                        }
                        else
                        {
                            PropertyInfo[] properties = typeof(Gun).GetProperties();
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
                                if (property.Name == "Icon" || property.Name == "Images" || property.Name == "Sounds")
                                    continue;
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
                        message = "SLIL console *v1.4*\nType \"-help-\" for a list of commands...";
                        console.Refresh();
                    }
                    else if (cheat == "SLC")
                    {
                        show_message = false;
                        previous_cheat.Clear();
                    }
                    else if (cheat.StartsWith("SAY "))
                    {
                        string[] say = cheat.Split(' ');
                        for (int i = 1; i < say.Length; i++)
                            message += say[i] + " ";
                    }
                    else if (cheat == "FPS")
                    {
                        parent.ShowFPS = !parent.ShowFPS;
                        INIReader.SetKey(MainMenu.iniFolder, "SLIL", "show_fps", parent.ShowFPS);
                        if (parent.ShowFPS)
                            message += "FPS display enabled.";
                        else
                            message += "FPS display disabled.";
                    }
                    else if (cheat == "MINIMAP")
                    {
                        parent.ShowMiniMap = !parent.ShowMiniMap;
                        INIReader.SetKey(MainMenu.iniFolder, "SLIL", "show_minimap", parent.ShowMiniMap);
                        if (parent.ShowMiniMap)
                            message += "Minimap enabled.";
                        else
                            message += "Minimap disabled.";
                    }
                    else if (cheat.StartsWith("VOL_"))
                    {
                        try
                        {
                            float x = Convert.ToSingle(cheat.Split('_')[1].Replace('.', ','));
                            if (x >= 0 && x <= 1)
                            {
                                message += $"Current volume is now {x}. *Default: 0,4*";
                                SLIL.Volume = x;
                                SLIL.SetVolume();
                            }
                            else
                            {
                                color = Color.Red;
                                message = "Incorrect value! X must be in the range from 0 to 1.";
                            }
                        }
                        catch
                        {
                            color = Color.Red;
                            message = "Incorrect data entered! X is not a number.";
                        }
                    }
                    else if (cheat.StartsWith("SCOPE_"))
                    {
                        try
                        {
                            int x = Convert.ToInt32(cheat.Split('_')[1]);
                            if (x > -1 && x < 5)
                            {
                                message += $"Current crosshair is now {x}. *Default: 0*";
                                SLIL.scope_type = x;
                                INIReader.SetKey(MainMenu.iniFolder, "SLIL", "scope_type", x);
                            }
                            else
                            {
                                color = Color.Red;
                                message = "Incorrect value! X must be in the range from 0 to 4.";
                            }
                        }
                        catch
                        {
                            color = Color.Red;
                            message = "Incorrect data entered! X is not a number.";
                        }
                    }
                    else if (cheat.StartsWith("SCOPECOL_"))
                    {
                        try
                        {
                            int x = Convert.ToInt32(cheat.Split('_')[1]);
                            if (x > -1 && x < 9)
                            {
                                message += $"Current crosshair color is now {x}. *Default: 0*";
                                SLIL.scope_color = x;
                                INIReader.SetKey(MainMenu.iniFolder, "SLIL", "scope_color", x);
                            }
                            else
                            {
                                color = Color.Red;
                                message = "Incorrect value! X must be in the range from 0 to 8.";
                            }
                        }
                        catch
                        {
                            color = Color.Red;
                            message = "Incorrect data entered! X is not a number.";
                        }
                    }
                    else if (cheat.StartsWith("LOOK_"))
                    {
                        try
                        {
                            double x = Convert.ToDouble(cheat.Split('_')[1].Replace('.', ','));
                            if (x < 2.5 || x > 10)
                            {
                                color = Color.Red;
                                message = "Incorrect range specified! Instead of X, enter a number between 2,5 and 10.";
                            }
                            else
                            {
                                message += $"Mouse sensitivity is now {x}. *Default: 2,75*";
                                SLIL.LOOK_SPEED = x;
                                INIReader.SetKey(MainMenu.iniFolder, "SLIL", "look_speed", x);
                            }
                        }
                        catch
                        {
                            color = Color.Red;
                            message = "Incorrect data entered! X is not a number.";
                        }
                    }
                    else if (cheat.StartsWith("COLOR_"))
                    {
                        try
                        {
                            int x = Convert.ToInt32(cheat.Split('_')[1]);
                            if (x < 0 || x > 10)
                            {
                                color = Color.Red;
                                message = "Incorrect range specified! Instead of X, enter a number between 0 and 10.";
                            }
                            else
                            {
                                color_index = x;
                                color = foreColors[color_index];
                                show_date = false;
                                console.Text = null;
                                message = "SLIL console *v1.4*\nType \"-help-\" for a list of commands...";
                                console.Refresh();
                            }
                        }
                        catch
                        {
                            color = Color.Red;
                            message = "Incorrect data entered! X is not a number.";
                        }
                    }
                    else if (!ImHonest)
                    {
                        if (cheat == "EFFECTS")
                        {
                            show_date = false;
                            message = "\n" +
                                 "~┌─────┬──────────────────┬──────────────────────────────┐~\n" +
                                 "~│~ *ID*  ~│~ *Effect*           ~│~ *Description*                  ~│~\n" +
                                 "~├─────┼──────────────────┼──────────────────────────────┤~\n";
                            for (int i = 0; i < effects.Length; i++)
                            {
                                string id = effects[i].ID.ToString().PadRight(4);
                                string name = effects[i].Name.PadRight(17);
                                string description = effects[i].Description.PadRight(29);
                                message += $"~│~ *{id}*~│~ -{name}-~│~ {description}~│~\n";
                            }
                            message += "~└─────┴──────────────────┴──────────────────────────────┘~";
                        }
                        else if (cheat == "EFCLEAR")
                        {
                            player.StopEffects();
                            message = "Effects have been cleared";
                        }
                        else if (cheat == "EFALLGV")
                        {
                            for (int i = 0; i < effects.Length; i++)
                            {
                                if (!player.EffectCheck(i))
                                    player.GiveEffect(i, true);
                            }
                            message = $"All effects have been issued";
                        }
                        else if (cheat.StartsWith("EFGIVE_"))
                        {
                            try
                            {
                                int x = Convert.ToInt32(cheat.Split('_')[1]);
                                if (x >= 0 && x < effects.Length)
                                {
                                    if (player.EffectCheck(x))
                                    {
                                        color = Color.Red;
                                        message = $"You already have an effect under ID {x}";
                                    }
                                    else
                                    {
                                        player.GiveEffect(x, true);
                                        message = $"The effect under ID {x} is issued";
                                    }
                                }
                                else
                                {
                                    color = Color.Red;
                                    message = $"There is no effect under ID {x}";
                                }
                            }
                            catch
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
                                if (x >= 0 && x < effects.Length)
                                {
                                    if (y < 5 || y > 9999)
                                    {
                                        color = Color.Red;
                                        message = "Incorrect value! Y must be in the range from 5 to 9999.";
                                    }
                                    else
                                    {
                                        player.GiveEffect(x, false, y);
                                        message = $"Effect at ID {x} was issued for {y} seconds";
                                    }
                                }
                                else
                                {
                                    color = Color.Red;
                                    message = $"There is no effect under ID {x}";
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
                        else if (cheat.StartsWith("CCHANC_"))
                        {
                            double x = 0.08;
                            bool error = false;
                            try
                            {
                                x = Convert.ToDouble(cheat.Split('_')[1].Replace('.', ','));
                            }
                            catch
                            {
                                error = true;
                            }
                            if (error || x < 0 || x > 1)
                            {
                                color = Color.Red;
                                message = "Incorrect range specified! Instead of X, enter a number between 0 and 1.";
                            }
                            else
                            {
                                message += $"Set chance of curse healing to {x * 100:0.##}% *Default: 8%*";
                                player.CurseCureChance = x;
                            }
                        }
                        else if (cheat.StartsWith("MONEY_"))
                        {
                            try
                            {
                                int x = Convert.ToInt32(cheat.Split('_')[1]);
                                message += $"The amount of money has been changed to {x}";
                                player.ChangeMoney(x);
                            }
                            catch
                            {
                                color = Color.Red;
                                message = "Incorrect data entered! X is not a number.";
                            }
                        }
                        else if (cheat.StartsWith("STAMIN_"))
                        {
                            try
                            {
                                int x = Convert.ToInt32(cheat.Split('_')[1]);
                                if (x >= 100 && x <= 5000)
                                {
                                    message += $"Player stamina is now {x}. *Default: 650*";
                                    player.MAX_STAMINE = x;
                                    player.STAMINE = x;
                                }
                                else
                                {
                                    color = Color.Red;
                                    message = "Incorrect value! X must be in the range from 100 to 5000.";
                                }
                            }
                            catch
                            {
                                color = Color.Red;
                                message = "Incorrect data entered! X is not a number.";
                            }
                        }
                        else if (cheat.StartsWith("SPEED_"))
                        {
                            try
                            {
                                double x = Convert.ToDouble(cheat.Split('_')[1]);
                                if (x >= 0.1 && x <= 20)
                                {
                                    message += $"Player speed is now {x}. *Default: 1,75*";
                                    player.MOVE_SPEED = x;
                                }
                                else
                                {
                                    color = Color.Red;
                                    message = "Incorrect value! X must be in the range from 0,1 to 20.";
                                }
                            }
                            catch
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
                            string[,] entities =
                            {
                                { "Player", "0" },
                                { "PlayerDeadBody", "1" },
                                { "Zombie", "2" },
                                { "Dog", "3" },
                                { "Abomination", "4" },
                                { "Bat", "5" },
                                { "Box", "6" },
                                { "Barrel", "7" },
                                { "ShopDoor", "8" },
                                { "ShopMan", "9" },
                                { "Teleport", "10" },
                                { "HittingTheWall", "11" },
                                { "RpgRocket", "12" },
                                { "Explosion", "13" },
                                { "SillyCat", "14" },
                                { "GreenGnome", "15" },
                                { "EnergyDrink", "16" },
                                { "Pyro", "17" },
                                { "Bike", "18" },
                            };
                            for (int i = 0; i < entities.GetLength(0); i++)
                            {
                                string id = entities[i, 1].PadRight(4);
                                string name = entities[i, 0].PadRight(17);
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
                                    if (x >= 0 && x < 19)
                                    {
                                        if (y < 0 || y > 1)
                                        {
                                            color = Color.Red;
                                            message = "Incorrect value! Y must be 0 or 1";
                                        }
                                        else
                                        {
                                            if (parent.SpawnEntity(x, y == 1))
                                            {
                                                if (y == 0) message = $"Creature with ID {x} successfully spawned with AI disabled";
                                                else message = $"Creature with ID {x} successfully spawned with AI enabled";
                                            }
                                            else
                                            {
                                                color = Color.Red;
                                                message = $"There was an error while spawning the enemy. Most likely a wall got in the way.";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        color = Color.Red;
                                        message = $"There is no entity under ID {x}";
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
                                try
                                {
                                    int x = Convert.ToInt32(cheat.Split('_')[1]);
                                    if (x >= 0 && x < 19)
                                    {
                                        if (parent.SpawnEntity(x, true))
                                            message = $"Creature with ID {x} successfully spawned with AI enabled";
                                        else
                                        {
                                            color = Color.Red;
                                            message = $"There was an error while spawning the enemy. Most likely a wall got in the way.";
                                        }
                                    }
                                    else
                                    {
                                        color = Color.Red;
                                        message = $"There is no entity under ID {x}";
                                    }
                                }
                                catch
                                {
                                    color = Color.Red;
                                    message = "Incorrect data entered! X or Y is not a number.";
                                }
                            }
                        }
                        else if (cheat == "NOCLIP")
                        {
                            if (parent.OnOffNoClip())
                                message = "noclip enabled";
                            else
                                message = "noclip disabled";
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
                            for (int i = 0; i < player.Guns.Count; i++)
                            {
                                if (player.GUNS[i].AddToShop)
                                    player.Guns[i].AmmoInStock = player.Guns[i].MaxAmmo;
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
                                player.GUNS[8].HasIt = true;
                                player.GUNS[8].AmmoInStock = player.GUNS[8].MaxAmmo;
                                if (!player.Guns.Contains(player.GUNS[8]))
                                    player.Guns.Add(player.GUNS[8]);
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
                                player.GUNS[9].HasIt = true;
                                player.GUNS[9].AmmoInStock = player.GUNS[9].MaxAmmo;
                                if (!player.Guns.Contains(player.GUNS[9]))
                                    player.Guns.Add(player.GUNS[9]);
                                message += "\"Wizard Gnome\" has been issued.";
                            }
                            else
                            {
                                color = Color.Red;
                                message = "Code not applied! You already have \"Wizard Gnome\"";
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
                        else if (cheat == "LPFJY")
                        {
                            player.HP = 1;
                            message += "Health reduced by 99.";
                        }
                        else
                        {
                            cheat_index = previous_cheat.Count - 1;
                            color = Color.Red;
                            message = $"Unknown command: {cheat}";
                        }
                    }
                    else
                    {
                        cheat_index = previous_cheat.Count - 1;
                        color = Color.Red;
                        message = $"Unknown command: {cheat}";
                    }
                    if (show_date)
                        time = $"\n-<{DateTime.Now:HH:mm}>- ";
                    if (show_message)
                        ConsoleAppendText($"\n{time}{message}", color);
                    if (color != Color.Red)
                    {
                        previous_cheat.Add(cheat);
                        cheat_index = previous_cheat.Count - 1;
                    }
                    ConsoleAppendText("\n\nEnter the command: ", foreColors[color_index]);
                    console.ScrollToCaret();
                }
            }
            else if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down)
            {
                if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                    return;
                if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                {
                    if (console.Text[console.Text.Length - 2] == ':' && console.Text[console.Text.Length - 1] == ' ') return;
                    if (command.Length > 0)
                    {
                        int start = command.Length - 1;
                        if (start < 0) start = 0;
                        command = command.Remove(start);
                    }
                    ConsoleDeleteText(1);
                    return;
                }
                char c = (char)e.KeyValue;
                if (!char.IsLetterOrDigit(c) && e.KeyCode != Keys.OemMinus && e.KeyCode != Keys.OemPeriod && e.KeyCode != Keys.Oemcomma && e.KeyCode != Keys.Space)
                    return;
                if (e.KeyCode == Keys.OemMinus) c = '_';
                else if (e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Oemcomma) c = ',';
                else if (e.KeyCode == Keys.Space) c = ' ';
                else if (e.KeyCode.ToString().StartsWith("Oem") || e.KeyCode == Keys.Divide || e.KeyCode == Keys.Subtract || e.KeyCode == Keys.Add)
                    return;
                command += c.ToString();
                ConsoleAppendColoredText(c.ToString(), Color.Cyan);
            }
            if (e.KeyCode == Keys.Up)
            {
                if (previous_cheat.Count == 0) return;
                ClearCommand();
                command = previous_cheat[cheat_index];
                cheat_index--;
                if (cheat_index < 0)
                    cheat_index = previous_cheat.Count - 1;
                ConsoleAppendColoredText(command, Color.Cyan);
            }
            if (e.KeyCode == Keys.Down)
            {
                if (previous_cheat.Count == 0) return;
                ClearCommand();
                command = previous_cheat[cheat_index];
                cheat_index++;
                if (cheat_index >= previous_cheat.Count)
                    cheat_index = 0;
                ConsoleAppendColoredText(command, Color.Cyan);
            }
        }

        public void ClearCommand()
        {
            ConsoleDeleteText(command.Length);
            command = "";
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
            string pattern = string.Join("|", colorMap.Keys.Select(k => $@"(\{k}.*?\{k})"));
            string[] parts = Regex.Split(text, pattern);
            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;
                var colorPair = colorMap.FirstOrDefault(pair => part.StartsWith(pair.Key) && part.EndsWith(pair.Key));
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
            cheat_index = previous_cheat.Count - 1;
        }

        private void Console_TextChanged(object sender, EventArgs e)
        {
            if (console.Text.Length == console.MaxLength)
            {
                console.Clear();
                ConsoleAppendText("SLIL console *v1.4*\nType \"-help-\" for a list of commands...", foreColors[color_index]);
                ConsoleAppendText("*The console was cleared due to a buffer overflow*", foreColors[color_index]);
                ConsoleAppendText("\n\nEnter the command: ", foreColors[color_index]);
                console.Refresh();
            }
        }

        public void Log(string message, bool newline, bool showTime, Color color)
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
            string pattern = @"screenshots\\screenshot_\d{4}_\d{2}_\d{2}__\d{2}_\d{2}_\d{2}\.png";
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
                        try
                        {
                            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                        }
                        catch (Exception ex)
                        {
                            Log($"Error opening file: {ex.Message}", true, true, Color.Red);
                        }
                    }
                    else
                        Log($"File not found: {filePath}", true, true, Color.Red);
                }
            }
        }

        private void Console_Load(object sender, EventArgs e) => console.Refresh();
    }
}