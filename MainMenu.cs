using System;
using System.IO;
using CGFReader;
using Play_Sound;
using System.Net;
using System.Text;
using System.Linq;
using SLIL.Classes;
using System.Drawing;
using SLIL.SLIL_v0_1;
using System.Net.Http;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;
using SLIL.SLIL_Localization;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SLIL
{
    internal partial class MainMenu : Form
    {
        private readonly string current_version = Program.current_version;
        internal bool UpdateVerified = false;
        internal bool downloadedLocalizationList = false;
        internal Localization localizations;
        internal Dictionary<string, string> supportedLanguages;
        internal static bool DownloadedLocalizationList = false;
        internal static Localization Localizations;
        private Dictionary<string, string> SupportedLanguages;
        internal static string Language = "English";
        internal string HashedKey = "None", License = "None";
        internal static bool sounds = true, ConsoleEnabled = false;
        internal TextureCache textureCache;
        internal static CGF_Reader CGFReader;
        private string SelectButtonName;
        private SLIL_Editor Editor;
        private bool ChangeControlButton = false, CanClose = false;
        private readonly PlaySound hmm, omg;
        private readonly PlaySound MainMenuTheme;
        private readonly PlaySound game_over;
        private readonly Dictionary<string, Keys> ClassicBindControls = new Dictionary<string, Keys>()
        {
            { "screenshot", Keys.F12 },
            { "reloading", Keys.R },
            { "forward", Keys.W },
            { "back", Keys.S },
            { "left", Keys.A },
            { "right", Keys.D },
            { "interaction_0", Keys.E },
            { "interaction_1", Keys.Enter },
            { "show_map_0", Keys.M },
            { "show_map_1", Keys.Tab },
            { "flashlight", Keys.F },
            { "item", Keys.H },
            { "select_item", Keys.Q },
            { "run", Keys.ShiftKey },
            { "climb", Keys.Space },
            { "inventory", Keys.I },
            { "kick", Keys.C },
        };
        internal static Dictionary<string, Keys> BindControls = new Dictionary<string, Keys>()
        {
            { "screenshot", Keys.F12 },
            { "reloading", Keys.R },
            { "forward", Keys.W },
            { "back", Keys.S },
            { "left", Keys.A },
            { "right", Keys.D },
            { "interaction_0", Keys.E },
            { "interaction_1", Keys.Enter },
            { "show_map_0", Keys.M },
            { "show_map_1", Keys.Tab },
            { "flashlight", Keys.F },
            { "item", Keys.H },
            { "select_item", Keys.Q },
            { "run", Keys.ShiftKey },
            { "climb", Keys.Space },
            { "inventory", Keys.I },
            { "kick", Keys.C },
        };
        internal static int smoothing = 1, scope_type = 0, scope_color = 0, interface_size = 2, difficulty = 2;
        internal static bool hight_fps = true, ShowFPS = false, ShowMiniMap = true, IsTutorial = false;
        internal static bool inv_y = false, inv_x = false;
        internal static double SLIL_v0_1_LOOK_SPEED = 1.75;
        internal static int SLIL_v0_1_difficulty = 1;
        internal static double LOOK_SPEED = 6.5;
        internal static float Volume = 0.4f, EffectsVolume = 0.4f, MusicVolume = 0.4f;
        internal static int Gamma = 100;
        private int ShowingSLIL_v0_0_1 = 0;

        internal MainMenu(CGF_Reader data, TextureCache textures)
        {
            InitializeComponent();
            Cursor = Program.SLILCursorDefault;
            version_label.Cursor = Program.SLILCursorHelp;
            CGFReader = data;
            textureCache = textures;
            if (!File.Exists("UpdateDownloader.exe"))
                DownloadFile("https://base-escape.ru/downloads/UpdateDownloader.exe", "UpdateDownloader.exe");
            if (!File.Exists("GameServer.exe"))
                DownloadFile("https://base-escape.ru/downloads/GameServer.exe", "GameServer.exe");
            MainMenuTheme = new PlaySound(CGFReader.GetFile("main_menu_theme.wav"), true);
            hmm = new PlaySound(CGFReader.GetFile("hmm.wav"), false);
            omg = new PlaySound(CGFReader.GetFile("OMG.wav"), false);
            game_over = new PlaySound(CGFReader.GetFile("game_over.wav"), false);
            AddSeparators();
        }

        //  #====  Developers Links  ====#

        private void Web_site_lonewolf_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://base-escape.ru") { UseShellExecute = true });

        private void Telegram_lonewolf_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://t.me/+VLJzjVRg8ElkZWYy") { UseShellExecute = true });

        private void Github_lonewolf_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/Lonewolf239") { UseShellExecute = true });

        private void Fatalan_git_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/Fatalan") { UseShellExecute = true });

        private void Qsvhu_telegram_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://t.me/Apsyuch") { UseShellExecute = true });

        private void Koyo_hipolink_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://hipolink.me/koyomichu") { UseShellExecute = true });

        private void Fazzy_telegram_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://t.me/Theripperofrice") { UseShellExecute = true });

        private void Darsin_tg_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://t.me/DARSINrock") { UseShellExecute = true });

        private void Maru_web_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("http://pornhub.com/") { UseShellExecute = true });

        //  #====      Updating      ====#

        private void Check_Update()
        {
            string title = "Update available!";
            string message = $"New update is out! Want to install it?\n\n" +
                          $"Current version: {current_version.Trim('|')}\n" +
                          $"Actual version: ";
            if (DownloadedLocalizationList)
            {
                message = $"{Localizations.GetLString(Language, "0-97")}\n\n" +
                             $"{Localizations.GetLString(Language, "0-98")} {current_version.Trim('|')}\n" +
                             $"{Localizations.GetLString(Language, "0-99")} ";
                title = Localizations.GetLString(Language, "0-96");
            }
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                webClient.DownloadStringCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        if (e.Error.HResult == -2146233079)
                        {
                            message = "Failed to establish a connection with the update server. Please check your internet connection.";
                            title = "Connection Error";
                            if (DownloadedLocalizationList)
                            {
                                message = Localizations.GetLString(Language, "0-101");
                                title = Localizations.GetLString(Language, "0-102");
                            }
                            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            message = $"An error occurred while downloading the update!";
                            title = $"Error";
                            if (DownloadedLocalizationList)
                            {
                                message = Localizations.GetLString(Language, "0-103");
                                title = Localizations.GetLString(Language, "0-94");
                            }
                            message += $" {e.Error.Message}.";
                            title += $" {e.Error.HResult}!";
                            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        UpdateVerified = false;
                        update_error_pic.Visible = true;
                        update_errors_background.Visible = true;
                        errors_panel.Visible = true;
                    }
                    else
                    {
                        if (e.Result.Length > 0 && !e.Result.Contains(current_version))
                        {
                            message += e.Result.Trim('|');
                            if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                if (!File.Exists("UpdateDownloader.exe"))
                                {
                                    message = "UpdateDownloader.exe has been deleted, renamed, or moved. After closing this message, it will be downloaded again.";
                                    string caption = "Error";
                                    if (DownloadedLocalizationList)
                                    {
                                        caption = Localizations.GetLString(Language, "0-94");
                                        message = Localizations.GetLString(Language, "0-95");
                                    }
                                    MessageBox.Show(message, caption, MessageBoxButtons.OK);
                                    DownloadFile("https://base-escape.ru/downloads/UpdateDownloader.exe", "UpdateDownloader.exe");
                                }
                                Process.Start(new ProcessStartInfo("UpdateDownloader.exe", "https://base-escape.ru/downloads/Setup_SLIL.exe Setup_SLIL"));
                                CanClose = true;
                                Application.Exit();
                            }
                        }
                        else
                        {
                            if (DownloadedLocalizationList)
                                MessageBox.Show(Localizations.GetLString(Language, "0-104"), Localizations.GetLString(Language, "0-105"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            else
                                MessageBox.Show("You already have the latest version of the SLIL installed.", "Version is current", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            UpdateVerified = true;
                            update_error_pic.Visible = false;
                            update_errors_background.Visible = false;
                        }
                    }
                };
                webClient.DownloadStringAsync(new Uri("https://base-escape.ru/version_SLIL.txt"));
            }
        }

        private void DownloadFile(string url, string outputPath)
        {
            using (WebClient client = new WebClient())
            {
                try { client.DownloadFile(new Uri(url), outputPath); }
                catch { }
            }
        }

        //  #====    Form methods    ====#

        private void AddSeparators()
        {
            all_settings.Controls.Clear();
            all_settings.Controls.Add(check_update_panel);
            all_settings.Controls.Add(new Separator());
            all_settings.Controls.Add(localization_panel);
            all_settings.Controls.Add(new Separator());
            all_settings.Controls.Add(language_panel);
            all_settings.Controls.Add(new Separator());
            all_settings.Controls.Add(console_panel);
            all_settings.Controls.Add(new Separator());
            all_settings.Controls.Add(show_tutorial_panel);
            sounds_settings.Controls.Clear();
            sounds_settings.Controls.Add(effects_volume_panel);
            sounds_settings.Controls.Add(new Separator());
            sounds_settings.Controls.Add(volume_panel);
            sounds_settings.Controls.Add(new Separator());
            sounds_settings.Controls.Add(music_volume_panel);
            sounds_settings.Controls.Add(new Separator());
            sounds_settings.Controls.Add(sound_panel);
            video_settings.Controls.Clear();
            video_settings.Controls.Add(interface_size_panel);
            video_settings.Controls.Add(new Separator());
            video_settings.Controls.Add(scope_color_panel);
            video_settings.Controls.Add(new Separator());
            video_settings.Controls.Add(scope_panel);
            video_settings.Controls.Add(new Separator());
            video_settings.Controls.Add(show_minimap_panel);
            video_settings.Controls.Add(new Separator());
            video_settings.Controls.Add(fps_panel);
            video_settings.Controls.Add(new Separator());
            video_settings.Controls.Add(gamma_panel);
            video_settings.Controls.Add(new Separator());
            video_settings.Controls.Add(show_fps_panel);
            video_settings.Controls.Add(new Separator());
            video_settings.Controls.Add(smoothing_panel);
            mouse_settings.Controls.Clear();
            mouse_settings.Controls.Add(invert_x_panel);
            mouse_settings.Controls.Add(new Separator());
            mouse_settings.Controls.Add(invert_y_panel);
            mouse_settings.Controls.Add(new Separator());
            mouse_settings.Controls.Add(sensitivity_panel);
            keyboard_settings.Controls.Clear();
            keyboard_settings.Controls.Add(kick_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(inventory_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(climb_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(run_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(select_item_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(medkit_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(flashlight_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(show_map_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(interaction_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(right_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(left_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(back_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(forward_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(reloading_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(aim_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(fire_panel);
            keyboard_settings.Controls.Add(new Separator());
            keyboard_settings.Controls.Add(screenshot_panel);
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            Activate();
            version_label.Text = $"v{current_version.Trim('|')}";
            SetLocalization();
            Language = GetLanguageName();
            GetGameParametrs();
            SetVisualSettings();
            SetLanguage();
            if (UpdateVerified)
            {
                update_error_pic.Visible = false;
                update_errors_background.Visible = false;
            }
            buttons_panel.Location = new Point((Width - buttons_panel.Width) / 2, (Height - buttons_panel.Height) / 2 + 75);
            difficulty_panel.Location = buttons_panel.Location;
            developers_panel.Location = buttons_panel.Location;
            settings_panel.Location = buttons_panel.Location;
            help_panel.Location = buttons_panel.Location;
            press_any_btn_panel.Location = buttons_panel.Location;
            exit_panel.Location = buttons_panel.Location;
            change_logs_panel.Location = buttons_panel.Location;
            account_panel.Location = buttons_panel.Location;
            hilf_mir_panel.Location = buttons_panel.Location;
            errors_panel.Location = new Point(buttons_panel.Left, Height - errors_panel.Height - 16);
            exit_size_panel.Left = (exit_panel.Width - exit_size_panel.Width) / 2;
            account_btn_c.Location = new Point(Width - account_btn_c.Width - 15, 15);
            SLIL_v0_1_btn_c.Location = new Point((Width - SLIL_v0_1_btn_c.Width) / 2, (Height - buttons_panel.Height) / 2 - 75);
            if (sounds) MainMenuTheme.Play(MusicVolume);
        }

        private void MainMenu_Shown(object sender, EventArgs e) { if (show_hilf_mir.Checked) hilf_mir_panel.Visible = true; }

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanClose)
            {
                e.Cancel = true;
                exit_panel.Visible = true;
                exit_panel.BringToFront();
                return;
            }
            secret_btn_timer.Stop();
            omg?.Dispose();
            MainMenuTheme?.Dispose();
            SaveSettings();
        }

        private void MainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            if (ChangeControlButton)
            {
                Keys key = e.KeyCode;
                if (key == Keys.Escape)
                {
                    ChangeControlButton = false;
                    press_any_btn_panel.Visible = false;
                    return;
                }
                if (BindControls.ContainsValue(key) || key.ToString().StartsWith("Oem") || key.ToString().StartsWith("Num") || (key.ToString().StartsWith("D") && key.ToString().Length == 2))
                {
                    cant_use_panel.Visible = true;
                    return;
                }
                if (BindControls.ContainsKey(SelectButtonName))
                {
                    BindControls[SelectButtonName] = key;
                    Button btn = Controls.Find(SelectButtonName + "_btn_c", true)[0] as Button;
                    btn.Text = key.ToString().Replace("Key", null).Replace("Return", "Enter");
                    ChangeControlButton = false;
                    press_any_btn_panel.Visible = false;
                    Program.iniReader.SetKey("SLIL", SelectButtonName, key);
                }
                e.Handled = true;
            }
        }

        private void MainMenu_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                lose_focus.Focus();
                if (developers_panel.Visible)
                    developers_panel.Visible = false;
                if (settings_panel.Visible)
                {
                    settings_panel.Visible = false;
                    SaveSettings();
                }
                if (exit_panel.Visible)
                    exit_panel.Visible = false;
                if (difficulty_panel.Visible)
                    difficulty_panel.Visible = false;
                if (change_logs_panel.Visible)
                    change_logs_panel.Visible = false;
                if (account_panel.Visible)
                {
                    account_panel.Visible = false;
                    SaveSettings();
                }
                if (hilf_mir_panel.Visible) hilf_mir_panel.Visible = false;
                if (help_panel.Visible) help_panel.Visible = false;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (exit_panel.Visible)
                {
                    CanClose = true;
                    Application.Exit();
                }
            }
        }

        private void ErrorsPanel_VisibleChanged(object sender, EventArgs e) => errors_panel.Visible = localization_errors_background.Visible || update_errors_background.Visible;

        //  #====   Game Parametrs   ====#

        private void GetGameParametrs()
        {
            show_hilf_mir.Checked = Program.iniReader.GetBool("CONFIG", "show_tutorial", true);
            show_tutorial.Checked = show_hilf_mir.Checked;
            Language = Program.iniReader.GetString("CONFIG", "language", Language);
            if (!SupportedLanguages.Values.Contains(Language)) Language = "English";
            ConsoleEnabled = Program.iniReader.GetBool("CONFIG", "console_enabled", ConsoleEnabled);
            sounds = Program.iniReader.GetBool("CONFIG", "sounds", true);
            MusicVolume = Program.iniReader.GetSingle("CONFIG", "music_volume", 0.4f);
            EffectsVolume = Program.iniReader.GetSingle("CONFIG", "effects_volume", 0.4f);
            Volume = Program.iniReader.GetSingle("CONFIG", "sounds_volume", 0.4f);
            Gamma = Program.iniReader.GetInt("CONFIG", "gamma", 100);
            LOOK_SPEED = Program.iniReader.GetDouble("SLIL", "look_speed", 6.5);
            inv_y = Program.iniReader.GetBool("SlIL", "inv_y", false);
            inv_x = Program.iniReader.GetBool("SlIL", "inv_x", false);
            ShowFPS = Program.iniReader.GetBool("SLIL", "show_fps", true);
            ShowMiniMap = Program.iniReader.GetBool("SLIL", "show_minimap", true);
            scope_color = Program.iniReader.GetInt("SLIL", "scope_color", 0);
            scope_type = Program.iniReader.GetInt("SLIL", "scope_type", 0);
            interface_size = Program.iniReader.GetInt("SLIL", "interface_size", 2);
            smoothing = Program.iniReader.GetInt("SLIL", "smoothing", 1);
            hight_fps = Program.iniReader.GetBool("SLIL", "hight_fps", true);
            BindControls["screenshot"] = Program.iniReader.GetKeys("HOTKEYS", "screenshot", Keys.F12);
            BindControls["reloading"] = Program.iniReader.GetKeys("HOTKEYS", "reloading", Keys.R);
            BindControls["forward"] = Program.iniReader.GetKeys("HOTKEYS", "forward", Keys.W);
            BindControls["back"] = Program.iniReader.GetKeys("HOTKEYS", "back", Keys.S);
            BindControls["left"] = Program.iniReader.GetKeys("HOTKEYS", "left", Keys.A);
            BindControls["right"] = Program.iniReader.GetKeys("HOTKEYS", "right", Keys.D);
            BindControls["interaction_0"] = Program.iniReader.GetKeys("HOTKEYS", "interaction_0", Keys.E);
            BindControls["interaction_1"] = Program.iniReader.GetKeys("HOTKEYS", "interaction_1", Keys.Enter);
            BindControls["show_map_0"] = Program.iniReader.GetKeys("HOTKEYS", "show_map_0", Keys.M);
            BindControls["show_map_1"] = Program.iniReader.GetKeys("HOTKEYS", "show_map_1", Keys.Tab);
            BindControls["flashlight"] = Program.iniReader.GetKeys("HOTKEYS", "flashlight", Keys.F);
            BindControls["item"] = Program.iniReader.GetKeys("HOTKEYS", "item", Keys.H);
            BindControls["select_item"] = Program.iniReader.GetKeys("HOTKEYS", "select_item", Keys.Q);
            BindControls["run"] = Program.iniReader.GetKeys("HOTKEYS", "run", Keys.ShiftKey);
            BindControls["climb"] = Program.iniReader.GetKeys("HOTKEYS", "climb", Keys.Space);
            BindControls["inventory"] = Program.iniReader.GetKeys("HOTKEYS", "inventory", Keys.I);
            BindControls["kick"] = Program.iniReader.GetKeys("HOTKEYS", "kick", Keys.C);
            SLIL_v0_1_LOOK_SPEED = Program.iniReader.GetDouble("SLIL_V0_0_1", "look_speed", 1.75);
            SLIL_v0_1_difficulty = Program.iniReader.GetInt("SLIL_V0_0_1", "difficulty", 1);
            interface_size = ML.Clamp(interface_size, 0, 3);
            smoothing = ML.Clamp(smoothing, 0, 3);
            LOOK_SPEED = ML.Clamp(LOOK_SPEED, 2.5, 10);
            MusicVolume = ML.Clamp(MusicVolume, 0, 1);
            EffectsVolume = ML.Clamp(EffectsVolume, 0, 1);
            Volume = ML.Clamp(Volume, 0, 1);
            Gamma = ML.Clamp(Gamma, 40, 120);
            scope_color = ML.Clamp(scope_color, 0, 8);
            scope_type = ML.Clamp(scope_type, 0, 4);
        }

        private void SaveSettings()
        {
            Program.iniReader.ClearFile();
            Program.iniReader.SetKey("ACCOUNT", "key", HashedKey);
            Program.iniReader.SetKey("ACCOUNT", "license", License);
            if (DownloadedLocalizationList) Program.iniReader.SetKey("CONFIG", "language", Language);
            Program.iniReader.SetKey("CONFIG", "sounds", sounds);
            Program.iniReader.SetKey("CONFIG", "console_enabled", ConsoleEnabled);
            Program.iniReader.SetKey("CONFIG", "show_tutorial", show_hilf_mir.Checked);
            Program.iniReader.SetKey("CONFIG", "music_volume", MusicVolume);
            Program.iniReader.SetKey("CONFIG", "effects_volume", EffectsVolume);
            Program.iniReader.SetKey("CONFIG", "sounds_volume", Volume);
            Program.iniReader.SetKey("CONFIG", "gamma", Gamma);
            Program.iniReader.SetKey("SLIL", "interface_size", interface_size);
            Program.iniReader.SetKey("SLIL", "smoothing", smoothing);
            Program.iniReader.SetKey("SLIL", "show_fps", ShowFPS);
            Program.iniReader.SetKey("SLIL", "hight_fps", hight_fps);
            Program.iniReader.SetKey("SLIL", "show_minimap", ShowMiniMap);
            Program.iniReader.SetKey("SLIL", "scope_type", scope_type);
            Program.iniReader.SetKey("SLIL", "scope_color", scope_color);
            Program.iniReader.SetKey("SLIL", "look_speed", LOOK_SPEED);
            Program.iniReader.SetKey("SLIL", "inv_y", inv_y);
            Program.iniReader.SetKey("SLIL", "inv_x", inv_x);
            Program.iniReader.SetKey("HOTKEYS", "screenshot", BindControls["screenshot"]);
            Program.iniReader.SetKey("HOTKEYS", "reloading", BindControls["reloading"]);
            Program.iniReader.SetKey("HOTKEYS", "forward", BindControls["forward"]);
            Program.iniReader.SetKey("HOTKEYS", "back", BindControls["back"]);
            Program.iniReader.SetKey("HOTKEYS", "left", BindControls["left"]);
            Program.iniReader.SetKey("HOTKEYS", "right", BindControls["right"]);
            Program.iniReader.SetKey("HOTKEYS", "interaction_0", BindControls["interaction_0"]);
            Program.iniReader.SetKey("HOTKEYS", "interaction_1", BindControls["interaction_1"]);
            Program.iniReader.SetKey("HOTKEYS", "show_map_0", BindControls["show_map_0"]);
            Program.iniReader.SetKey("HOTKEYS", "show_map_1", BindControls["show_map_1"]);
            Program.iniReader.SetKey("HOTKEYS", "flashlight", BindControls["flashlight"]);
            Program.iniReader.SetKey("HOTKEYS", "item", BindControls["item"]);
            Program.iniReader.SetKey("HOTKEYS", "select_item", BindControls["select_item"]);
            Program.iniReader.SetKey("HOTKEYS", "run", BindControls["run"]);
            Program.iniReader.SetKey("HOTKEYS", "climb", BindControls["climb"]);
            Program.iniReader.SetKey("HOTKEYS", "inventory", BindControls["inventory"]);
            Program.iniReader.SetKey("HOTKEYS", "kick", BindControls["kick"]);
            Program.iniReader.SetKey("SLIL_V0_0_1", "look_speed", SLIL_v0_1_LOOK_SPEED);
            Program.iniReader.SetKey("SLIL_V0_0_1", "difficulty", difficulty);
        }

        //  #====    Localization    ====#

        private async Task DownloadLocalizationList()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    string result = await httpClient.GetStringAsync("https://base-escape.ru/SLILLocalization/LocalizationList.txt");
                    string[] lines = result.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    string[] languages = new string[lines.Length];
                    string[] codes = new string[lines.Length];
                    if (lines.Length > 0)
                    {
                        for (int i = 0; i < lines.Length; i++)
                        {
                            codes[i] = lines[i].Split('-')[0];
                            languages[i] = lines[i].Split('-')[1];
                        }
                        language_list.Items.Clear();
                        language_list.Items.AddRange(languages.Distinct().ToArray());
                        await SetLocalizations(codes, languages);
                    }
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ConnectFailure)
                        MessageBox.Show("Failed to establish a connection with the localizations server. Please check your internet connection.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show($"An error occurred while downloading the localization: {e.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DownloadedLocalizationList = false;
                    return;
                }
                catch (Exception e)
                {
                    MessageBox.Show($"An unexpected error occurred: {e.Message}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DownloadedLocalizationList = false;
                    return;
                }
            }
            Localizations.RemoveDuplicates();
        }

        internal void SetLocalization()
        {
            DownloadedLocalizationList = downloadedLocalizationList;
            SupportedLanguages = supportedLanguages;
            if (DownloadedLocalizationList)
            {
                language_list.Items.Clear();
                language_list.Items.AddRange(SupportedLanguages.Values.Distinct().ToArray());
            }
            Localizations = localizations;
        }

        private async Task SetLocalizations(string[] codes, string[] languages)
        {
            SupportedLanguages.Clear();
            for (int i = 0; i < languages.Length; i++)
                SupportedLanguages.Add(codes[i], languages[i]);
            for (int i = 0; i < languages.Length; i++)
            {
                try
                {
                    await Localizations.DownloadLocalization(languages[i]);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    DownloadedLocalizationList = false;
                    return;
                }
            }
            DownloadedLocalizationList = true;
        }

        private string GetBtnName(string name) => BindControls[name].ToString().Replace("Key", null).Replace("Return", "Enter");

        private void SetVisualSettings()
        {
            screenshot_btn_c.Text = GetBtnName("screenshot");
            reloading_btn_c.Text = GetBtnName("reloading");
            forward_btn_c.Text = GetBtnName("forward");
            back_btn_c.Text = GetBtnName("back");
            left_btn_c.Text = GetBtnName("left");
            right_btn_c.Text = GetBtnName("right");
            interaction_0_btn_c.Text = GetBtnName("interaction_0");
            interaction_1_btn_c.Text = GetBtnName("interaction_1");
            show_map_0_btn_c.Text = GetBtnName("show_map_0");
            show_map_1_btn_c.Text = GetBtnName("show_map_1");
            flashlight_btn_c.Text = GetBtnName("flashlight");
            item_btn_c.Text = GetBtnName("item");
            select_item_btn_c.Text = GetBtnName("select_item");
            run_btn_c.Text = GetBtnName("run");
            climb_btn_c.Text = GetBtnName("climb");
            inventory_btn_c.Text = GetBtnName("inventory");
            kick_btn_c.Text = GetBtnName("kick");
            language_list.SelectedIndex = language_list.Items.IndexOf(Language);
            show_tutorial.Checked = show_hilf_mir.Checked;
            localization_error_pic.Visible = !DownloadedLocalizationList;
            errors_panel.Visible = localization_errors_background.Visible = !DownloadedLocalizationList;
            interface_size_choice.Value = interface_size;
            smoothing_list.SelectedIndex = smoothing;
            console_btn.Checked = ConsoleEnabled;
            sounds_on_off.Checked = sounds;
            show_fps_on_off.Checked = ShowFPS;
            fps.Value = hight_fps ? 1 : 0;
            show_minimap.Checked = ShowMiniMap;
            scope_choice.Value = scope_type;
            scope_color_choice.Value = scope_color;
            gamma_choice.Value = Gamma;
            sensitivity.Value = (int)(LOOK_SPEED * 100);
            if (sounds)
            {
                music_volume.Value = (int)(MusicVolume * 100);
                effects_volume.Value = (int)(EffectsVolume * 100);
                volume.Value = (int)(Volume * 100);
                music_volume.Enabled = effects_volume.Enabled = volume.Enabled = true;
            }
            else
            {
                music_volume.Value = effects_volume.Value = volume.Value = 0;
                music_volume.Enabled = effects_volume.Enabled = volume.Enabled = false;
            }
            if (hight_fps) fps_label.Text = "FPS: 60";
            else fps_label.Text = "FPS: 30";
            if (DownloadedLocalizationList)
            {
                scope_label.Text = Localizations.GetLString(Language, "0-36") + GetScopeType();
                scope_color_label.Text = Localizations.GetLString(Language, "0-37") + GetScopeColor();
                interface_size_label.Text = Localizations.GetLString(Language, "0-108") + GetInterfaceSize();
            }
            else
            {
                scope_label.Text = "Scope: " + GetScopeType();
                scope_color_label.Text = "Scope color: " + GetScopeColor();
                interface_size_label.Text = "Interface size: " + GetInterfaceSize();
            }
        }

        private string GetLanguageName()
        {
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string languageCode = ci.Name.ToLower();
            if (SupportedLanguages.ContainsKey(languageCode))
                return SupportedLanguages[languageCode];
            string shortCode = languageCode.Substring(0, 2);
            if (SupportedLanguages.ContainsKey(shortCode))
                return SupportedLanguages[shortCode];
            return "English";
        }

        private void SetLanguage()
        {
            smoothing_list.Items.Clear();
            if (DownloadedLocalizationList)
            {
                string[] smooth_list =
                {
                    Localizations.GetLString(Language, "0-61"),
                    Localizations.GetLString(Language, "0-62"),
                    Localizations.GetLString(Language, "0-63"),
                    Localizations.GetLString(Language, "0-64")
                };
                smoothing_list.Items.AddRange(smooth_list);
                kick_label.Text = Localizations.GetLString(Language, "0-134");
                inventory_label.Text = Localizations.GetLString(Language, "0-130");
                localization_update_btn.Text = Localizations.GetLString(Language, "0-90");
                smoothing_label.Text = Localizations.GetLString(Language, "0-1");
                console_label.Text = Localizations.GetLString(Language, "0-2");
                nickname_label.Text = Localizations.GetLString(Language, "0-3");
                help_close_l.Text = Localizations.GetLString(Language, "0-6");
                start_btn_cp.Text = Localizations.GetLString(Language, "0-8");
                easy_btn.Text = Localizations.GetLString(Language, "0-11");
                normal_btn.Text = Localizations.GetLString(Language, "0-12");
                hard_btn.Text = Localizations.GetLString(Language, "0-13");
                very_hard_btn.Text = Localizations.GetLString(Language, "0-14");
                custom_btn.Text = Localizations.GetLString(Language, "0-15");
                close_difficulty_panel_l.Text = Localizations.GetLString(Language, "0-6");
                start_game_btn_r.Text = Localizations.GetLString(Language, "0-7");
                setting_btn_cp.Text = Localizations.GetLString(Language, "0-16");
                sounds_settings.Text = Localizations.GetLString(Language, "0-133");
                music_volume_label.Text = Localizations.GetLString(Language, "0-131");
                effects_volume_label.Text = Localizations.GetLString(Language, "0-132");
                volume_label.Text = Localizations.GetLString(Language, "0-17");
                about_developers_btn_cp.Text = Localizations.GetLString(Language, "0-18");
                open_help_btn_cp.Text = Localizations.GetLString(Language, "0-19");
                bug_repor_btn_cp.Text = Localizations.GetLString(Language, "0-89");
                create_translate_cp.Text = Localizations.GetLString(Language, "0-88");
                exit_btn_cp.Text = Localizations.GetLString(Language, "0-20");
                exit_label.Text = Localizations.GetLString(Language, "0-21");
                exit_yes_btn_c.Text = Localizations.GetLString(Language, "0-22");
                exit_no_btn_c.Text = Localizations.GetLString(Language, "0-23");
                fatalan_about.Text = Localizations.GetLString(Language, "0-24");
                qsvhu_about.Text = Localizations.GetLString(Language, "0-25");
                koyo_about.Text = Localizations.GetLString(Language, "0-26");
                fazzy_about.Text = Localizations.GetLString(Language, "0-106");
                close_developers_r.Text = Localizations.GetLString(Language, "0-10");
                all_settings.Text = Localizations.GetLString(Language, "0-27");
                sounds_label.Text = Localizations.GetLString(Language, "0-28");
                language_label.Text = Localizations.GetLString(Language, "0-29");
                check_update_btn.Text = Localizations.GetLString(Language, "0-31");
                video_settings.Text = Localizations.GetLString(Language, "0-32");
                gamma_label.Text = Localizations.GetLString(Language, "0-121") + $" {GetGamma()}";
                show_fps_label.Text = Localizations.GetLString(Language, "0-34");
                show_minimap_label.Text = Localizations.GetLString(Language, "0-35");
                scope_label.Text = Localizations.GetLString(Language, "0-36") + GetScopeType();
                scope_color_label.Text = Localizations.GetLString(Language, "0-37") + GetScopeColor();
                controls_settings.Text = Localizations.GetLString(Language, "0-38");
                keyboard_settings.Text = Localizations.GetLString(Language, "0-39");
                mouse_settings.Text = Localizations.GetLString(Language, "0-40");
                sensitivity_label.Text = Localizations.GetLString(Language, "0-41");
                invert_y_label.Text = Localizations.GetLString(Language, "0-42");
                invert_x_label.Text = Localizations.GetLString(Language, "0-43");
                clear_settings_l.Text = Localizations.GetLString(Language, "0-44");
                close_settings_r.Text = Localizations.GetLString(Language, "0-10");
                change_logs_close_btn_r.Text = Localizations.GetLString(Language, "0-10");
                hilf_mir_close_btn_r.Text = Localizations.GetLString(Language, "0-10");
                show_hilf_mir.Text = Localizations.GetLString(Language, "0-116");
                show_tutorial_label.Text = Localizations.GetLString(Language, "0-116");
                tutorial_label.Text = Localizations.GetLString(Language, "0-117");
                tutorial_btn_cp.Text = Localizations.GetLString(Language, "0-118");
                go_tutorial_btn_cp.Text = Localizations.GetLString(Language, "0-119");
                press_any_btn_label.Text = Localizations.GetLString(Language, "0-45");
                cant_use_panel.Text = Localizations.GetLString(Language, "0-107");
                interface_size_label.Text = Localizations.GetLString(Language, "0-108") + GetInterfaceSize();
                screenshot_label.Text = Localizations.GetLString(Language, "0-46");
                fire_btn_c.Text = Localizations.GetLString(Language, "0-47");
                aim_btn_c.Text = Localizations.GetLString(Language, "0-48");
                reloading_label.Text = Localizations.GetLString(Language, "0-49");
                fire_label.Text = Localizations.GetLString(Language, "0-50");
                aim_label.Text = Localizations.GetLString(Language, "0-51");
                forward_label.Text = Localizations.GetLString(Language, "0-52");
                back_label.Text = Localizations.GetLString(Language, "0-6");
                left_label.Text = Localizations.GetLString(Language, "0-53");
                right_label.Text = Localizations.GetLString(Language, "0-54");
                interaction_label.Text = Localizations.GetLString(Language, "0-55");
                show_map_label.Text = Localizations.GetLString(Language, "0-56");
                flashlight_label.Text = Localizations.GetLString(Language, "0-57");
                medkit_label.Text = Localizations.GetLString(Language, "0-58");
                select_item_label.Text = Localizations.GetLString(Language, "0-59");
                run_label.Text = Localizations.GetLString(Language, "0-60");
                climb_label.Text = Localizations.GetLString(Language, "0-120");
                update_error_label.Text = Localizations.GetLString(Language, "0-122");
                localization_error_label.Text = Localizations.GetLString(Language, "0-123");
                darsin_about.Text = Localizations.GetLString(Language, "0-124");
                maru_about.Text = Localizations.GetLString(Language, "0-125");
            }
            else
            {
                smoothing_list.Items.AddRange(new string[] { "No Antialiasing", "Default", "High Quality", "High Speed" });
                kick_label.Text = "Kick";
                inventory_label.Text = "Inventory";
                kick_label.Text = "Kick";
                localization_update_btn.Text = "Update language list";
                smoothing_label.Text = "Smoothing";
                console_label.Text = "Developer console";
                nickname_label.Text = "Player name:";
                help_close_l.Text = "Back";
                start_btn_cp.Text = "Start game";
                easy_btn.Text = "Easy";
                normal_btn.Text = "Normal";
                hard_btn.Text = "Difficult";
                very_hard_btn.Text = "Very difficult";
                custom_btn.Text = "Editor";
                close_difficulty_panel_l.Text = "Return";
                start_game_btn_r.Text = "Play";
                sounds_settings.Text = "Sounds";
                music_volume_label.Text = "Music volume";
                effects_volume_label.Text = "Effects volume";
                volume_label.Text = "Volume";
                setting_btn_cp.Text = "Settings";
                about_developers_btn_cp.Text = "About developers";
                open_help_btn_cp.Text = "Feedback";
                bug_repor_btn_cp.Text = "Report a bug";
                create_translate_cp.Text = "Add localization";
                exit_btn_cp.Text = "Exit game";
                exit_label.Text = "Do you really want to exit?";
                exit_yes_btn_c.Text = "Yes";
                exit_no_btn_c.Text = "No";
                fatalan_about.Text = "Texturing, sprite rendering, enemy and pet AI";
                qsvhu_about.Text = "Weapon sprites and sounds";
                koyo_about.Text = "Textures, enemy sprites and menu background";
                fazzy_about.Text = "Translation of the game into Ukrainian";
                close_developers_r.Text = "Close";
                all_settings.Text = "General";
                sounds_label.Text = "Game sounds";
                language_label.Text = "Language";
                check_update_btn.Text = "Check for update";
                video_settings.Text = "Graphics";
                gamma_label.Text = "Gamma " + GetGamma();
                show_fps_label.Text = "Display FPS";
                show_minimap_label.Text = "Show minimap";
                scope_label.Text = "Scope: " + GetScopeType();
                scope_color_label.Text = "Scope color: " + GetScopeColor();
                controls_settings.Text = "Control";
                keyboard_settings.Text = "Keyboard";
                mouse_settings.Text = "Mouse";
                sensitivity_label.Text = "Mouse sensitivity";
                invert_y_label.Text = "Invert Y-axis";
                invert_x_label.Text = "Invert X-axis";
                clear_settings_l.Text = "Reset";
                close_settings_r.Text = "Close";
                change_logs_close_btn_r.Text = "Close";
                hilf_mir_close_btn_r.Text = "Close";
                show_hilf_mir.Text = "Show tutorial";
                show_tutorial_label.Text = "Show tutorial";
                tutorial_label.Text = "It seems you are a beginner.\nWould you like to take a training course?";
                tutorial_btn_cp.Text = "Complete training";
                go_tutorial_btn_cp.Text = "Training";
                press_any_btn_label.Text = "Press any button or ESC to cancel";
                cant_use_panel.Text = "This button can't be used!";
                interface_size_label.Text = "Interface size: " + GetInterfaceSize();
                screenshot_label.Text = "Screenshot";
                fire_btn_c.Text = "LMB";
                aim_btn_c.Text = "RMB";
                reloading_label.Text = "Reloading";
                fire_label.Text = "Shot";
                aim_label.Text = "Aiming";
                forward_label.Text = "Forward";
                back_label.Text = "Backward";
                left_label.Text = "Left";
                right_label.Text = "Right";
                interaction_label.Text = "Interaction";
                show_map_label.Text = "Show/hide map";
                flashlight_label.Text = "Flashlight";
                medkit_label.Text = "Use item";
                select_item_label.Text = "Select item";
                run_label.Text = "Run (hold)";
                climb_label.Text = "Climb over";
                update_error_label.Text = "Error checking for update";
                localization_error_label.Text = "Error loading localization";
                darsin_about.Text = "Creating death sounds for enemies, motorcycles, and others";
                maru_about.Text = "Author of dibal ideas";
            }
            if (show_tutorial.Checked)
            {
                if (DownloadedLocalizationList)
                    show_tutorial.Text = Localizations.GetLString(Language, "0-70");
                else
                    show_tutorial.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    show_tutorial.Text = Localizations.GetLString(Language, "0-71");
                else
                    show_tutorial.Text = "Off";
            }
            if (ConsoleEnabled)
            {
                if (DownloadedLocalizationList)
                    console_btn.Text = Localizations.GetLString(Language, "0-70");
                else
                    console_btn.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    console_btn.Text = Localizations.GetLString(Language, "0-71");
                else
                    console_btn.Text = "Off";
            }
            if (sounds)
            {
                if (DownloadedLocalizationList)
                    sounds_on_off.Text = Localizations.GetLString(Language, "0-70");
                else
                    sounds_on_off.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    sounds_on_off.Text = Localizations.GetLString(Language, "0-71");
                else
                    sounds_on_off.Text = "Off";
            }
            if (ShowFPS)
            {
                if (DownloadedLocalizationList)
                    show_fps_on_off.Text = Localizations.GetLString(Language, "0-70");
                else
                    show_fps_on_off.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    show_fps_on_off.Text = Localizations.GetLString(Language, "0-71");
                else
                    show_fps_on_off.Text = "Off";
            }
            if (ShowMiniMap)
            {
                if (DownloadedLocalizationList)
                    show_minimap.Text = Localizations.GetLString(Language, "0-70");
                else
                    show_minimap.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    show_minimap.Text = Localizations.GetLString(Language, "0-71");
                else
                    show_minimap.Text = "Off";
            }
            if (inv_y)
            {
                if (DownloadedLocalizationList)
                    invert_y.Text = Localizations.GetLString(Language, "0-70");
                else
                    invert_y.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    invert_y.Text = Localizations.GetLString(Language, "0-71");
                else
                    invert_y.Text = "Off";
            }
            if (inv_x)
            {
                if (DownloadedLocalizationList)
                    invert_x.Text = Localizations.GetLString(Language, "0-70");
                else
                    invert_x.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    invert_x.Text = Localizations.GetLString(Language, "0-71");
                else
                    invert_x.Text = "Off";
            }
            smoothing_list.SelectedIndex = smoothing;
            GetDifficulty();
            ResizeButtonsInControl(this);
        }

        private string GetScopeType()
        {
            switch (scope_type)
            {
                case 0:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-72");
                    return "Standard";
                case 1:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-73");
                    return "Cross";
                case 2:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-74");
                    return "Line";
                case 3:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-75");
                    return "Dot";
                default:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-76");
                    return "No scope";
            }
        }

        private string GetScopeColor()
        {
            switch (scope_color)
            {
                case 0:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-77");
                    return "Green";
                case 1:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-78");
                    return "Red";
                case 2:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-79");
                    return "Yellow";
                case 3:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-80");
                    return "Blue";
                case 4:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-81");
                    return "Purple";
                case 5:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-82");
                    return "Cyan";
                case 6:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-83");
                    return "Orange";
                case 7:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-84");
                    return "White";
                default:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-85");
                    return "Random";
            }
        }

        private string GetGamma() => $"{Gamma}%";

        private string GetInterfaceSize()
        {
            switch (interface_size)
            {
                case 0:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-109");
                    return "Very small";
                case 1:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-110");
                    return "Small";
                case 2:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-111");
                    return "Normal";
                default:
                    if (DownloadedLocalizationList)
                        return " " + Localizations.GetLString(Language, "0-112");
                    return "Big";
            }
        }

        private void GetDifficulty()
        {
            switch (difficulty)
            {
                case 0:
                    if (DownloadedLocalizationList)
                        difficulty_label.Text = Localizations.GetLString(Language, "0-65") + Localizations.GetLString(Language, "0-113");
                    else
                        difficulty_label.Text = "Starting weapon: Pistol Lvl 1\nEnemies respawn every 60 seconds\nEnemies deal 25% more damage";
                    break;
                case 1:
                    if (DownloadedLocalizationList)
                        difficulty_label.Text = Localizations.GetLString(Language, "0-66") + Localizations.GetLString(Language, "0-114");
                    else
                        difficulty_label.Text = "Starting weapon: Pistol Lvl 1\nEnemies deal 25% more damage";
                    break;
                case 2:
                    if (DownloadedLocalizationList)
                        difficulty_label.Text = Localizations.GetLString(Language, "0-67") + Localizations.GetLString(Language, "0-129");
                    else
                        difficulty_label.Text = "Starting weapon: Pistol Lvl 2\nYou will have 2 first aid kits";
                    break;
                case 3:
                    if (DownloadedLocalizationList)
                        difficulty_label.Text = Localizations.GetLString(Language, "0-68") + Localizations.GetLString(Language, "0-115") + Localizations.GetLString(Language, "0-129");
                    else
                        difficulty_label.Text = "Starting weapon: Pistol lvl 2\nEnemies give more money\nEnemies deal 25% less damage\nYou will have 2 first aid kits";
                    break;
                default:
                    if (DownloadedLocalizationList)
                        difficulty_label.Text = Localizations.GetLString(Language, "0-69");
                    else
                        difficulty_label.Text = "Level parameters can be changed manually";
                    break;
            }
        }

        private void ResizeButtonsInControl(Control control)
        {
            if (control is Button button && !control.Name.EndsWith("_c"))
            {
                button.Size = new Size(0, 0);
                if (button.Name.EndsWith("_r"))
                    button.Left = button.Parent.Width - button.Width - 7;
                else if (button.Name.EndsWith("_l"))
                    button.Left = 7;
                else if (button.Name.EndsWith("_cp"))
                    button.Left = (button.Parent.Width - button.Width) / 2;
            }
            foreach (Control childControl in control.Controls)
                ResizeButtonsInControl(childControl);
        }

        //  #====    Close Buttons   ====#

        private void Close_developers_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            developers_panel.Visible = false;
        }

        private void Close_settings_Click(object sender, EventArgs e)
        {
            settings_panel.Visible = false;
            SaveSettings();
        }

        private void Exit_no_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            exit_panel.Visible = false;
        }

        private void Exit_yes_btn_Click(object sender, EventArgs e)
        {
            CanClose = true;
            Application.Exit();
        }

        private void Close_difficulty_panel_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            difficulty_panel.Visible = false;
        }

        private void Change_logs_close_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            change_logs_panel.Visible = false;
        }

        private void Close_account_btn_c_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            account_panel.Visible = false;
            SaveSettings();
        }

        private void Hilf_mir_close_btn_r_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            hilf_mir_panel.Visible = false;
        }

        private void Help_close_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            help_panel.Visible = false;
        }

        //  #====    Menu Buttons    ====#

        private void Start_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            IsTutorial = false;
            normal_btn.Checked = true;
            difficulty_panel.Visible = true;
            difficulty_panel.BringToFront();
        }

        private void Setting_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            settings_panel.Visible = true;
            settings_panel.BringToFront();
        }

        private void About_developers_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            developers_panel.Visible = true;
            developers_panel.BringToFront();
        }

        private void Bug_report_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            help_panel.Visible = true;
            help_panel.BringToFront();
        }

        private void Exit_btn_Click(object sender, EventArgs e)
        {
            exit_panel.Visible = true;
            exit_panel.BringToFront();
        }

        //  #====      Settings      ====#

        private void Clear_settings_Click(object sender, EventArgs e)
        {
            show_hilf_mir.Checked = true;
            Language = GetLanguageName();
            sounds = true;
            ConsoleEnabled = false;
            interface_size = 2;
            ShowFPS = false;
            hight_fps = true;
            ShowMiniMap = true;
            scope_type = 0;
            scope_color = 0;
            LOOK_SPEED = 6.5;
            MusicVolume = EffectsVolume = Volume = 0.4f;
            Gamma = 100;
            foreach (var kvp in ClassicBindControls)
                BindControls[kvp.Key] = kvp.Value;
            SetVisualSettings();
        }

        //  #==      All Settings      ==#

        private void Show_tutorial_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            show_hilf_mir.Checked = show_tutorial.Checked;
            if (show_tutorial.Checked)
            {
                if (DownloadedLocalizationList)
                    show_tutorial.Text = Localizations.GetLString(Language, "0-70");
                else
                    show_tutorial.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    show_tutorial.Text = Localizations.GetLString(Language, "0-71");
                else
                    show_tutorial.Text = "Off";
            }
        }

        private void Console_btn_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            ConsoleEnabled = console_btn.Checked;
            if (ConsoleEnabled)
            {
                if (DownloadedLocalizationList)
                    console_btn.Text = Localizations.GetLString(Language, "0-70");
                else
                    console_btn.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    console_btn.Text = Localizations.GetLString(Language, "0-71");
                else
                    console_btn.Text = "Off";
            }
        }

        private void Sounds_on_off_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            sounds = sounds_on_off.Checked;
            if (sounds)
            {
                if (DownloadedLocalizationList)
                    sounds_on_off.Text = Localizations.GetLString(Language, "0-70");
                else
                    sounds_on_off.Text = "On";
                MainMenuTheme.Play(MusicVolume);
            }
            else
            {
                if (DownloadedLocalizationList)
                    sounds_on_off.Text = Localizations.GetLString(Language, "0-71");
                else
                    sounds_on_off.Text = "Off";
                MainMenuTheme.Stop();
            }
            SetVisualSettings();
        }

        private void Music_volume_Scroll(object sender, EventArgs e)
        {
            MusicVolume = (float)music_volume.Value / 100;
            MainMenuTheme.SetVolume(MusicVolume);
        }
        private void Effects_volume_Scroll(object sender, EventArgs e) => EffectsVolume = (float)effects_volume.Value / 100;

        private void Volume_Scroll(object sender, EventArgs e) => Volume = (float)volume.Value / 100;

        private void Language_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            Language = language_list.SelectedItem.ToString();
            SetLanguage();
        }

        private async void Localization_update_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            DownloadedLocalizationList = false;
            Language = Program.iniReader.GetString("CONFIG", "language", Language);
            await DownloadLocalizationList();
            SetVisualSettings();
            SetLanguage();
        }

        private void Check_update_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            Check_Update();
        }

        //  #==     Video Settings     ==#

        private void Smoothing_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            smoothing = smoothing_list.SelectedIndex;
        }

        private void Show_fps_on_off_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            ShowFPS = show_fps_on_off.Checked;
            if (ShowFPS)
            {
                if (DownloadedLocalizationList)
                    show_fps_on_off.Text = Localizations.GetLString(Language, "0-70");
                else
                    show_fps_on_off.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    show_fps_on_off.Text = Localizations.GetLString(Language, "0-71");
                else
                    show_fps_on_off.Text = "Off";
            }
        }

        private void Gamma_choice_Scroll(object sender, EventArgs e)
        {
            Gamma = gamma_choice.Value;
            if (DownloadedLocalizationList)
                gamma_label.Text = Localizations.GetLString(Language, "0-121") + $" {GetGamma()}";
            else
                gamma_label.Text = "Gamma " + GetGamma();
        }

        private void Fps_Scroll(object sender, EventArgs e)
        {
            hight_fps = fps.Value == 1;
            if (hight_fps) fps_label.Text = "FPS: 60";
            else fps_label.Text = "FPS: 30";
        }

        private void Show_minimap_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            ShowMiniMap = show_minimap.Checked;
            if (ShowMiniMap)
            {
                if (DownloadedLocalizationList)
                    show_minimap.Text = Localizations.GetLString(Language, "0-70");
                else
                    show_minimap.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    show_minimap.Text = Localizations.GetLString(Language, "0-71");
                else
                    show_minimap.Text = "Off";
            }
        }

        private void Scope_choice_Scroll(object sender, EventArgs e)
        {
            scope_type = scope_choice.Value;
            if (DownloadedLocalizationList)
                scope_label.Text = Localizations.GetLString(Language, "0-36") + GetScopeType();
            else
                scope_label.Text = "Scope: " + GetScopeType();
        }

        private void Scope_color_choice_Scroll(object sender, EventArgs e)
        {
            scope_color = scope_color_choice.Value;
            if (DownloadedLocalizationList)
                scope_color_label.Text = Localizations.GetLString(Language, "0-37") + GetScopeColor();
            else
                scope_color_label.Text = "Scope color: " + GetScopeColor();
        }

        private void Interface_size_choice_Scroll(object sender, EventArgs e)
        {
            interface_size = interface_size_choice.Value;
            if (DownloadedLocalizationList)
                interface_size_label.Text = Localizations.GetLString(Language, "0-108") + GetInterfaceSize();
            else
                interface_size_label.Text = "Interface size: " + GetInterfaceSize();
        }

        //  #==    Control Settings    ==#

        private void Sensitivity_Scroll(object sender, EventArgs e) => LOOK_SPEED = (double)sensitivity.Value / 100;

        private void Invert_y_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            inv_y = invert_y.Checked;
            if (inv_y)
            {
                if (DownloadedLocalizationList)
                    invert_y.Text = Localizations.GetLString(Language, "0-70");
                else
                    invert_y.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    invert_y.Text = Localizations.GetLString(Language, "0-71");
                else
                    invert_y.Text = "Off";
            }
        }

        private void Invert_x_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            inv_x = invert_x.Checked;
            if (inv_x)
            {
                if (DownloadedLocalizationList)
                    invert_x.Text = Localizations.GetLString(Language, "0-70");
                else
                    invert_x.Text = "On";
            }
            else
            {
                if (DownloadedLocalizationList)
                    invert_x.Text = Localizations.GetLString(Language, "0-71");
                else
                    invert_x.Text = "Off";
            }
        }

        private void ChangeControl_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            if (!ChangeControlButton)
            {
                ChangeControlButton = true;
                SelectButtonName = (sender as Button).Name.Replace("_btn_c", null);
                cant_use_panel.Visible = false;
                press_any_btn_panel.Visible = true;
                press_any_btn_panel.BringToFront();
            }
        }

        //  #====      Game mode     ====#

        private void Difficulty_CheckedChanged(object sender, EventArgs e)
        {
            if (custom_btn.Checked)
            {
                difficulty = 4;
                if (DownloadedLocalizationList)
                    start_game_btn_r.Text = Localizations.GetLString(Language, "0-15");
                else
                    start_game_btn_r.Text = "Editor";
            }
            else
            {
                if (DownloadedLocalizationList)
                    start_game_btn_r.Text = Localizations.GetLString(Language, "0-7");
                else
                    start_game_btn_r.Text = "Play";
                if (easy_btn.Checked)
                    difficulty = 3;
                else if (normal_btn.Checked)
                    difficulty = 2;
                else if (hard_btn.Checked)
                    difficulty = 1;
                else
                    difficulty = 0;
            }
            start_game_btn_r.Size = new Size(0, 0);
            start_game_btn_r.Left = start_game_btn_r.Parent.Width - start_game_btn_r.Width - 7;
            GetDifficulty();
        }

        private void Go_tutorial_btn_cp_Click(object sender, EventArgs e) => GoToTutorial();

        private void Fade_timer_Tick(object sender, EventArgs e)
        {
            float volume = MainMenuTheme.GetVolume();
            if (volume <= 0.01)
            {
                fade_timer.Stop();
                MainMenuTheme?.Stop();
                MainMenuTheme.SetVolume(MusicVolume);
            }
            else
            {
                volume -= 0.01f;
                MainMenuTheme.SetVolume(volume);
            }
        }

        private void Start_game_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            if (difficulty != 4)
            {
                if (sounds) fade_timer.Start();
                SLIL form = new SLIL(textureCache) { PlayerName = "Player" };
                form.ShowDialog();
                if (sounds) MainMenuTheme.Play(MusicVolume);
                difficulty_panel.Visible = false;
            }
            else
            {
                MainMenuTheme?.Stop();
                Editor = new SLIL_Editor
                {
                    Owner = this,
                    MazeHeight = 13,
                    MazeWidth = 13
                };
                Editor.FormClosing += EditorForm_FormClosing;
                Editor.ShowDialog();
                if (sounds) MainMenuTheme.SetVolume(MusicVolume);
            }
        }

        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Editor != null && Editor.OK)
            {
                MainMenuTheme.Stop();
                SLIL form = new SLIL(textureCache, true, Editor.MAP, (Editor.MazeWidth - 1) / 3, (Editor.MazeHeight - 1) / 3, SLIL_Editor.x, SLIL_Editor.y) { PlayerName = "Player" };
                form.ShowDialog();
                if (sounds) MainMenuTheme.Play(MusicVolume);
                Editor = null;
                difficulty_panel.Visible = false;
            }
        }

        //  #====     Change Logs    ====#

        private void Version_label_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                change_logs_panel.Visible = true;
                change_logs_panel.BringToFront();
            }
        }

        private void Changes_list_Enter(object sender, EventArgs e) => lose_focus.Focus();

        private void Change_logs_panel_VisibleChanged(object sender, EventArgs e)
        {
            if (changes_list.Visible)
            {
                changes_list.Items.Clear();
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        string[] changes = client.DownloadString($"https://base-escape.ru/SLILLocalization/Changelogs/{Language}.txt").Replace("@", "\t\t\tv").Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        changes_list.Items.AddRange(changes);
                    }
                }
                catch (Exception er)
                {
                    changes_list.Items.Clear();
                    if (DownloadedLocalizationList)
                        changes_list.Items.Add(Localizations.GetLString(Language, "0-91"));
                    else
                        changes_list.Items.Add("Error getting changelog");
                    changes_list.Items.Add(er.Message);
                }
            }
        }

        //  #====      Feedback      ====#

        private void Bug_repor_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            Process.Start(new ProcessStartInfo("https://t.me/SLILBugReportBOT") { UseShellExecute = true });
            help_panel.Visible = false;
        }

        private void Create_translate_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            Process.Start(new ProcessStartInfo("https://t.me/SLILLocalizationBOT") { UseShellExecute = true });
            help_panel.Visible = false;
        }

        //  #====      Account       ====#

        private void Account_btn_c_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            Process.Start(new ProcessStartInfo("https://t.me/SLIL_AccountBOT?account") { UseShellExecute = true });
            //nickname.Text = PlayerName;
            //account_panel.Visible = true;
            //account_panel.BringToFront();
        }

        //  #====      Tutorial      ====#

        private void Tutorial_btn_cp_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            GoToTutorial();
        }

        private void GoToTutorial()
        {
            if (sounds) fade_timer.Start();
            StringBuilder tutorialMap = new StringBuilder(
            "#########################" +
            "#.....######.....########" +
            "#.....######.b.B.####...#" +
            "#..P..######.....####.F.#" +
            "#.....######.....####...#" +
            "#.....###$####.#######.##" +
            "###d#####D####d#S###S#d##" +
            "##.........#............#" +
            "##.........d............#" +
            "##.........#............#" +
            "###S#S#S#S##.......######" +
            "############.......######" +
            "#........d.........d....#" +
            "#.......##.........##...#" +
            "#..##=#.#S.........S#...#" +
            "#.....#.######d#S####...#" +
            "#.......######.######...#" +
            "#.#.....##.........##...#" +
            "#.#=##..##.........##...#" +
            "#.#.....##.........##...#" +
            "#.......##.........##...#" +
            "#...E...##.1.2.3.4.##...#" +
            "#.E...E.##.........##.e.#" +
            "#.E...E.##.........##...#" +
            "#########################");
            IsTutorial = true;
            difficulty = 4;
            SLIL form = new SLIL(textureCache, true, tutorialMap, 8, 8, 3.5, 3.5) { PlayerName = "Player" };
            form.ShowDialog();
            hilf_mir_panel.Visible = difficulty_panel.Visible = false;
            if (sounds) MainMenuTheme.Play(MusicVolume);
        }

        private void Show_hilf_mir_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            show_tutorial.Checked = show_hilf_mir.Checked;
            Program.iniReader.SetKey("CONFIG", "show_tutorial", show_hilf_mir.Checked);
        }

        //  #====      SLIL v0.0.1     ====#

        private const int TotalSecretTicks = 500;
        private int CurrentSecretTick = 0;
        private double initialLeft, initialTop, initialWidth, initialHeight;
        private double finalLeft, finalTop;
        private double deltaLeft, deltaTop, deltaWidth, deltaHeight;

        private void Secret_panel_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            if (ShowingSLIL_v0_0_1 == 10) 
            {
                hmm.Stop();
                omg.Play(1);
                initialWidth = Width;
                initialHeight = Height;
                initialLeft = 0;
                initialTop = -initialHeight;
                SLIL_v0_1_btn_c.Size = new Size((int)initialWidth, (int)initialHeight);
                SLIL_v0_1_btn_c.Location = new Point((int)initialLeft, (int)initialTop);
                finalLeft = (Width - 301) / 2;
                finalTop = (Height - buttons_panel.Height) / 2 - 75;
                deltaLeft = (finalLeft - initialLeft) / TotalSecretTicks;
                deltaTop = (finalTop - initialTop) / TotalSecretTicks;
                deltaWidth = (301 - initialWidth) / TotalSecretTicks;
                deltaHeight = (47 - initialHeight) / TotalSecretTicks;
                CurrentSecretTick = 0;
                slil_0_0_1_dev_panel.Visible = false;
                developers_panel.Visible = false;
                SLIL_v0_1_btn_c.Visible = true;
                SLIL_v0_1_btn_c.BringToFront();
                secret_btn_timer.Start();
                ShowingSLIL_v0_0_1++;
            }
            else if (ShowingSLIL_v0_0_1 < 10)
            {
                ShowingSLIL_v0_0_1++;
                hmm.Play(0.5f);
            }
        }

        private void Secret_btn_timer_Tick(object sender, EventArgs e)
        {
            if (CurrentSecretTick >= TotalSecretTicks)
            {
                SLIL_v0_1_btn_c.Size = new Size((int)301, (int)47);
                SLIL_v0_1_btn_c.Location = new Point((int)finalLeft, (int)finalTop);
                secret_btn_timer.Stop();
                return;
            }
            CurrentSecretTick++;
            double newLeft = initialLeft + deltaLeft * CurrentSecretTick;
            double newTop = initialTop + deltaTop * CurrentSecretTick;
            double newWidth = initialWidth + deltaWidth * CurrentSecretTick;
            double newHeight = initialHeight + deltaHeight * CurrentSecretTick;
            SLIL_v0_1_btn_c.Size = new Size((int)newWidth, (int)newHeight);
            SLIL_v0_1_btn_c.Location = new Point((int)newLeft, (int)newTop);
        }

        private void SLIL_v0_1_btn_c_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            if (secret_btn_timer.Enabled) return;
            MainMenuTheme.Stop();
            new SLILv0_1() { game_over = game_over }.ShowDialog();
            if (sounds) MainMenuTheme.Play(MusicVolume);
        }
    }
}