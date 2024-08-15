using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CGFReader;
using System.Globalization;
using System;
using IniReader;
using System.Net;
using System.Diagnostics;
using SLIL.Classes;
using Play_Sound;

namespace SLIL
{
    public partial class MainMenu : Form
    {
        private const string current_version = "|1.1.1a|";
        public static string iniFolder = "config.ini";
        public static bool Language = true, sounds = true, ConsoleEnabled = false;
        private readonly TextureCache textureCache = new TextureCache();
        public static CGF_Reader CGFReader;
        private string SelectButtonName;
        private SLIL_Editor Editor;
        private bool ChangeControlButton = false, CanClose = false;
        private readonly PlaySound[] ost;
        private readonly PlaySound MainMenuTheme;
        private readonly PlaySound[,] step;
        private readonly PlaySound[,] DeathSounds;
        private readonly PlaySound[,] CuteDeathSounds;
        private readonly PlaySound game_over, draw, buy, hit, hungry, wall, tp, screenshot;
        private readonly PlaySound[] door;
        private readonly Pet[] PETS;
        public static Player player;
        private readonly string[] en_changes =
        {
            "\t\t\tv1.1.1a",
            "_______________________________________________________________",
            "• Critical error fixed",
            "\n",
            "\t\t\tv1.1.1",
            "_______________________________________________________________",
            "• Added 2 new effects: Regeneration and Protection",
            "• Medkit has been reworked",
            "• Podseratel and Adrenaline have been balanced",
            "• Fixed some bugs",
            "\n",
            "\t\t\tv1.1",
            "_______________________________________________________________",
            "• Podseratel: Added ability",
            "• Added new item: Adrenaline",
            "• Improved optimization",
            "• Difficulty rebalance",
            "• Added music to main menu",
            "• Fixed some bugs",
            "\n",
            "\t\t\tv1.0.2",
            "_______________________________________________________________",
            "• Fixed detection of the run key",
            "• Some bugs fixed",
            "\n",
            "\t\t\tv1.0.1",
            "_______________________________________________________________",
            "• Fixed game over screen",
            "\n",
            "\t\t\tv1.0",
            "_______________________________________________________________",
            "• Game Release",
            "• SLIL was separated from Mini-Games"
        };
        private readonly string[] ru_changes =
        {
            "\t\t\tv1.1.1a",
            "_______________________________________________________________",
            "• Исправлена ​​критическая ошибка",
            "\n",
            "\t\t\tv1.1.1",
            "_______________________________________________________________",
            "• Добавлены 2 новых эффекта: Регенерация и Защита",
            "• Аптечка была переработана",
            "• Подсератель и Адреналин были сбалансированы",
            "• Исправлены некоторые ошибки",
            "\n",
            "\t\t\tv1.1",
            "_______________________________________________________________",
            "• Подсератель: Добавлена ​​способность",
            "• Добавлен новый предмет: Адреналин",
            "• Улучшена оптимизация",
            "• Ребаланс сложности",
            "• Добавлена ​​музыка в главное меню",
            "• Исправлены некоторые ошибки",
            "\n",
            "\t\t\tv1.0.2",
            "_______________________________________________________________",
            "• Исправлено обнаружение клавиши бега",
            "• Исправлены некоторые ошибки",
            "\n",
            "\t\t\tv1.0.1",
            "_______________________________________________________________",
            "• Исправлен экран окончания игры",
            "\n",
            "\t\t\tv1.0",
            "_______________________________________________________________",
            "• Релиз игры",
            "• SLIL был отделен от Mini-Games"
        };
        private readonly string[,] AboutDifficulty =
        {
            {
                // Очень сложно
                "Начальное оружие: Пистолет 1 ур.\nВраги возрождаются каждые 60 секунд",
                // Сложно
                "Начальное оружие: Пистолет 1 ур.",
                // Нормально
                "Начальное оружие: Пистолет 2 ур.",
                // Легко
                "Начальное оружие: Пистолет 2 ур.\nВраги дают больше денег",
                // Редактор
                "Параметры уровня могут быть изменены вручную"
            },
            {
                // Очень сложно
                "Starting weapon: Pistol Lv1\nEnemies respawn every 60 seconds",
                // Сложно
                "Starting weapon: Pistol Lv1",
                // Нормально
                "Starting weapon: Pistol Lv2",
                // Легко
                "Starting weapon: Pistol Lv2\nEnemies give more money",
                // Редактор
                "Level parameters can be manually adjusted"
            }
        };
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
        };
        public static Dictionary<string, Keys> BindControls = new Dictionary<string, Keys>()
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
        };
        public static int resolution = 0, scope_type = 0, scope_color = 0, difficulty = 2;
        public static bool hight_fps = true, ShowFPS = true, ShowMiniMap = true;
        public static double LOOK_SPEED = 6.5;
        public static float Volume = 0.4f;

        public MainMenu()
        {
            InitializeComponent();
            if (File.Exists("data.cgf"))
                CGFReader = new CGF_Reader("data.cgf");
            else
            {
                string title = "Отсутствует файл \"data.cgf\"!", message = $"Файл \"data.cgf\" отсутствует! Возможно, он был переименован, перемещен или удален. Хотите загрузить установщик еще раз?";
                if (!Check_Language())
                {
                    title = "Missing \"data.cgf\" file!";
                    message = $"The file \"data.cgf\" is missing! It may have been renamed, moved, or deleted. Do you want to download the installer again?";
                }
                if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    Hide();
                    WindowState = FormWindowState.Minimized;
                    Downloading _form = new Downloading
                    {
                        update = false,
                        language = Check_Language()
                    };
                    _form.ShowDialog();
                }
                else
                    Application.Exit();
            }
            //PETS = new Pet[] { new SillyCat(0, 0, 0, 0), new GreenGnome(0, 0, 0), new EnergyDrink(0, 0, 0), new Pyro(0, 0, 0) };
            //player = new Player(1.5, 1.5, 15);
            ost = new PlaySound[]
            {
                new PlaySound(CGFReader.GetFile("slil_ost_0.wav"), true),
                new PlaySound(CGFReader.GetFile("slil_ost_1.wav"), true),
                new PlaySound(CGFReader.GetFile("slil_ost_2.wav"), true),
                new PlaySound(CGFReader.GetFile("slil_ost_3.wav"), true),
                new PlaySound(CGFReader.GetFile("slil_ost_4.wav"), true),
                new PlaySound(CGFReader.GetFile("soul_forge.wav"), true),
                new PlaySound(CGFReader.GetFile("gnome.wav"), true),
                new PlaySound(CGFReader.GetFile("cmode_ost.wav"), true)
            };
            MainMenuTheme = new PlaySound(CGFReader.GetFile("main_menu_theme.wav"), true);
            step = new PlaySound[,]
            {
                {
                    new PlaySound(CGFReader.GetFile("step_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_1.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_2.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_3.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_4.wav"), false)
                },
                {
                    new PlaySound(CGFReader.GetFile("step_run_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_run_1.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_run_2.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_run_3.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_run_4.wav"), false)
                },
                {
                    new PlaySound(CGFReader.GetFile("step_c_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_c_1.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_c_2.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_c_3.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_c_4.wav"), false)
                },
                {
                    new PlaySound(CGFReader.GetFile("step_run_c_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_run_c_1.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_run_c_2.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_run_c_3.wav"), false),
                    new PlaySound(CGFReader.GetFile("step_run_c_4.wav"), false)
                }
            };
            DeathSounds = new PlaySound[,]
            {
                //Zombie
                {
                    new PlaySound(CGFReader.GetFile("zombie_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("zombie_die_1.wav"), false),
                    new PlaySound(CGFReader.GetFile("zombie_die_2.wav"), false)
                },
                //Dog
                {
                    new PlaySound(CGFReader.GetFile("dog_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("dog_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("dog_die_0.wav"), false)
                },
                //Abomination
                {
                    new PlaySound(CGFReader.GetFile("abomination_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("abomination_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("abomination_die_0.wav"), false)
                },
                //Bat
                {
                    new PlaySound(CGFReader.GetFile("bat_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("bat_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("bat_die_0.wav"), false)
                }
            };
            CuteDeathSounds = new PlaySound[,]
            {
                //Zombie
                {
                    new PlaySound(CGFReader.GetFile("c_zombie_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("c_zombie_die_1.wav"), false),
                    new PlaySound(CGFReader.GetFile("c_zombie_die_0.wav"), false)
                },
                //Dog
                {
                    new PlaySound(CGFReader.GetFile("c_dog_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("c_dog_die_1.wav"), false),
                    new PlaySound(CGFReader.GetFile("c_dog_die_0.wav"), false)
                },
                //Abomination
                {
                    new PlaySound(CGFReader.GetFile("c_abomination_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("c_abomination_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("c_abomination_die_0.wav"), false)
                },
                //Bat
                {
                    new PlaySound(CGFReader.GetFile("c_bat_die_0.wav"), false),
                    new PlaySound(CGFReader.GetFile("c_bat_die_1.wav"), false),
                    new PlaySound(CGFReader.GetFile("c_bat_die_0.wav"), false)
                }
            };
            game_over = new PlaySound(CGFReader.GetFile("game_over.wav"), false);
            draw = new PlaySound(CGFReader.GetFile("draw.wav"), false);
            buy = new PlaySound(CGFReader.GetFile("buy.wav"), false);
            hit = new PlaySound(CGFReader.GetFile("hit_player.wav"), false);
            hungry = new PlaySound(CGFReader.GetFile("hungry_player.wav"), false);
            wall = new PlaySound(CGFReader.GetFile("wall_interaction.wav"), false);
            tp = new PlaySound(CGFReader.GetFile("tp.wav"), false);
            screenshot = new PlaySound(CGFReader.GetFile("screenshot.wav"), false);
            door = new PlaySound[] { new PlaySound(CGFReader.GetFile("door_opened.wav"), false), new PlaySound(CGFReader.GetFile("door_closed.wav"), false) };
        }

        private void Bug_report_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            Process.Start(new ProcessStartInfo("https://t.me/MiniGamesBugReport_BOT") { UseShellExecute = true });
        }

        private void Web_site_lonewolf_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://base-escape.ru") { UseShellExecute = true });

        private void Telegram_lonewolf_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://t.me/+VLJzjVRg8ElkZWYy") { UseShellExecute = true });

        private void Github_lonewolf_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/Lonewolf239") { UseShellExecute = true });

        private void Fatalan_git_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/Fatalan") { UseShellExecute = true });

        private void Qsvhu_telegram_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://t.me/Apsyuch") { UseShellExecute = true });

        private void Koyo_hipolink_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://hipolink.me/koyomichu") { UseShellExecute = true });

        private void Close_developers_Click(object sender, EventArgs e) => developers_panel.Visible = false;

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanClose)
            {
                e.Cancel = true;
                exit_panel.Visible = true;
                exit_panel.BringToFront();
                return;
            }
            SaveSettings();
        }

        private void Close_settings_Click(object sender, EventArgs e)
        {
            SaveSettings();
            settings_panel.Visible = false;
        }

        private void Exit_btn_Click(object sender, EventArgs e)
        {
            exit_panel.Visible = true;
            exit_panel.BringToFront();
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

        private bool Check_Language()
        {
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string language = ci.Name.ToLower();
            string[] supportedLanguages = { "ru", "uk", "be", "kk", "ky" };
            return Array.Exists(supportedLanguages, lang => lang.Equals(language) || lang.Equals(language.Substring(0, 2)));
        }

        private void Check_Update(bool auto)
        {
            string title = "Доступно обновление!";
            string message = $"Вышло новое обновление! Хотите установить его?\n\n" +
                             $"Текущая версия: {current_version.Trim('|')}\n" +
                             $"Актуальная версия: ";
            string update_text = "\n\nСписок изменений:";
            if (!Check_Language())
            {
                title = "Update available!";
                message = $"New update is out! Want to install it?\n\n" +
                          $"Current version: {current_version.Trim('|')}\n" +
                          $"Actual version: ";
                update_text = "\n\nList of changes:";
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
                            if (!auto)
                            {
                                message = "Не удалось установить соединение с сервером обновлений. Проверьте подключение к интернету.";
                                title = "Ошибка подключения";
                                if (!Language)
                                {
                                    message = "Failed to establish a connection with the update server. Please check your internet connection.";
                                    title = "Connection Error";
                                }
                                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            if (!auto)
                            {
                                message = $"Во время загрузки обновления произошла ошибка! {e.Error.Message}.";
                                title = $"Ошибка {e.Error.HResult}!";
                                if (!Language)
                                {
                                    message = $"An error occurred while downloading the update! {e.Error.Message}.";
                                    title = $"Error {e.Error.HResult}!";
                                }
                                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        string[] lines = e.Result.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                        if (lines.Length > 0 && !lines[0].Contains(current_version))
                        {
                            message += lines[0].Trim('|');
                            message += update_text;
                            bool isRussian = Check_Language();
                            for (int i = 1; i < lines.Length; i++)
                            {
                                string line = lines[i].Trim();
                                if (line.StartsWith("ru:"))
                                {
                                    if (isRussian)
                                        message += "\n• " + line.Substring(3);
                                }
                                else if (line.StartsWith("en:"))
                                {
                                    if (!isRussian)
                                        message += "\n• " + line.Substring(3);
                                }
                            }
                            if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                Hide();
                                WindowState = FormWindowState.Minimized;
                                Downloading _form = new Downloading
                                {
                                    update = true,
                                    language = Check_Language()
                                };
                                _form.ShowDialog();
                            }
                        }
                        else
                        {
                            if (!auto)
                            {
                                if (Language)
                                    MessageBox.Show("У вас уже установлена последняя версия программы.", "Версия актуальная", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                else
                                    MessageBox.Show("You already have the latest version of the program installed.", "Version is current", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                };
                webClient.DownloadStringAsync(new Uri("https://base-escape.ru/version_SLIL.txt"));
            }
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            Activate();
            version_label.Text = $"v{current_version.Trim('|')}";
            Language = Check_Language();
            INIReader.CreateIniFileIfNotExist(iniFolder);
            GetGameParametrs();
            SetVisualSettings();
            SetLanguage();
            if (update_on_off.Checked)
                Check_Update(true);
            buttons_panel.Location = new Point((Width - buttons_panel.Width) / 2, (Height - buttons_panel.Height) / 2 + 75);
            difficulty_panel.Location = buttons_panel.Location;
            developers_panel.Location = buttons_panel.Location;
            settings_panel.Location = buttons_panel.Location;
            press_any_btn_panel.Location = buttons_panel.Location;
            exit_panel.Location = buttons_panel.Location;
            change_logs_panel.Location = buttons_panel.Location;
            game_mode_panel.Location = buttons_panel.Location;
            multiplayer_panel.Location = buttons_panel.Location;
            host_panel.Location = buttons_panel.Location;
            connect_panel.Location = buttons_panel.Location;
            exit_size_panel.Left = (exit_panel.Width - exit_size_panel.Width) / 2;
            if (sounds) MainMenuTheme.Play(Volume);
        }

        private void GetGameParametrs()
        {
            Language = INIReader.GetBool(iniFolder, "CONFIG", "language", Language);
            ConsoleEnabled = INIReader.GetBool(iniFolder, "CONFIG", "console_enabled", ConsoleEnabled);
            sounds = INIReader.GetBool(iniFolder, "CONFIG", "sounds", true);
            update_on_off.Checked = INIReader.GetBool(iniFolder, "CONFIG", "auto_update", true);
            LOOK_SPEED = INIReader.GetDouble(iniFolder, "SLIL", "look_speed", 6.5);
            Volume = INIReader.GetSingle(iniFolder, "SLIL", "volume", 0.4f);
            ShowFPS = INIReader.GetBool(iniFolder, "SLIL", "show_fps", true);
            ShowMiniMap = INIReader.GetBool(iniFolder, "SLIL", "show_minimap", true);
            scope_color = INIReader.GetInt(iniFolder, "SLIL", "scope_color", 0);
            scope_type = INIReader.GetInt(iniFolder, "SLIL", "scope_type", 0);
            resolution = INIReader.GetBool(iniFolder, "SLIL", "hight_resolution", false) ? 1 : 0;
            hight_fps = INIReader.GetBool(iniFolder, "SLIL", "hight_fps", true);
            BindControls["screenshot"] = INIReader.GetKeys(iniFolder, "SLIL", "screenshot", Keys.F12);
            BindControls["reloading"] = INIReader.GetKeys(iniFolder, "SLIL", "reloading", Keys.R);
            BindControls["forward"] = INIReader.GetKeys(iniFolder, "SLIL", "forward", Keys.W);
            BindControls["back"] = INIReader.GetKeys(iniFolder, "SLIL", "back", Keys.S);
            BindControls["left"] = INIReader.GetKeys(iniFolder, "SLIL", "left", Keys.A);
            BindControls["right"] = INIReader.GetKeys(iniFolder, "SLIL", "right", Keys.D);
            BindControls["interaction_0"] = INIReader.GetKeys(iniFolder, "SLIL", "interaction_0", Keys.E);
            BindControls["interaction_1"] = INIReader.GetKeys(iniFolder, "SLIL", "interaction_1", Keys.Enter);
            BindControls["show_map_0"] = INIReader.GetKeys(iniFolder, "SLIL", "show_map_0", Keys.M);
            BindControls["show_map_1"] = INIReader.GetKeys(iniFolder, "SLIL", "show_map_1", Keys.Tab);
            BindControls["flashlight"] = INIReader.GetKeys(iniFolder, "SLIL", "flashlight", Keys.F);
            BindControls["item"] = INIReader.GetKeys(iniFolder, "SLIL", "item", Keys.H);
            BindControls["select_item"] = INIReader.GetKeys(iniFolder, "SLIL", "select_item", Keys.Q);
            BindControls["run"] = INIReader.GetKeys(iniFolder, "SLIL", "run", Keys.ShiftKey);
            if (LOOK_SPEED < 2.5 || LOOK_SPEED > 10)
                LOOK_SPEED = 6.5;
            if (Volume < 0 || Volume > 1)
                Volume = 0.4f;
            if (scope_color < 0 || scope_color > 8)
                scope_color = 0;
            if (scope_type < 0 || scope_type > 4)
                scope_type = 0;
        }

        private void SetVisualSettings()
        {
            screenshot_btn.Text = BindControls["screenshot"].ToString().Replace("Key", null).Replace("Return", "Enter");
            reloading_btn.Text = BindControls["reloading"].ToString().Replace("Key", null).Replace("Return", "Enter");
            forward_btn.Text = BindControls["forward"].ToString().Replace("Key", null).Replace("Return", "Enter");
            back_btn.Text = BindControls["back"].ToString().Replace("Key", null).Replace("Return", "Enter");
            left_btn.Text = BindControls["left"].ToString().Replace("Key", null).Replace("Return", "Enter");
            right_btn.Text = BindControls["right"].ToString().Replace("Key", null).Replace("Return", "Enter");
            interaction_0_btn.Text = BindControls["interaction_0"].ToString().Replace("Key", null).Replace("Return", "Enter");
            interaction_1_btn.Text = BindControls["interaction_1"].ToString().Replace("Key", null).Replace("Return", "Enter");
            show_map_0_btn.Text = BindControls["show_map_0"].ToString().Replace("Key", null).Replace("Return", "Enter");
            show_map_1_btn.Text = BindControls["show_map_1"].ToString().Replace("Key", null).Replace("Return", "Enter");
            flashlight_btn.Text = BindControls["flashlight"].ToString().Replace("Key", null).Replace("Return", "Enter");
            item_btn.Text = BindControls["item"].ToString().Replace("Key", null).Replace("Return", "Enter");
            select_item_btn.Text = BindControls["select_item"].ToString().Replace("Key", null).Replace("Return", "Enter");
            run_btn.Text = BindControls["run"].ToString().Replace("Key", null).Replace("Return", "Enter");
            language_list.SelectedIndex = Language ? 0 : 1;
            console_btn.Checked = ConsoleEnabled;
            sounds_on_off.Checked = sounds;
            high_resolution_on_off.Checked = resolution == 1;
            show_fps_on_off.Checked = ShowFPS;
            fps.Value = hight_fps ? 1 : 0;
            show_minimap.Checked = ShowMiniMap;
            scope_choice.Value = scope_type;
            scope_color_choice.Value = scope_color;
            sensitivity.Value = (int)(LOOK_SPEED * 100);
            volume.Value = (int)(Volume * 100);
            if (hight_fps)
                fps_label.Text = "FPS: 60";
            else
                fps_label.Text = "FPS: 30";
            if (Language)
            {
                scope_label.Text = "Прицел: " + GetScopeType();
                scope_color_label.Text = "Цвет прицела: " + GetScopeColor();
            }
            else
            {
                scope_label.Text = "Scope: " + GetScopeType();
                scope_color_label.Text = "Scope color: " + GetScopeColor();
            }
        }

        private void SetLanguage()
        {
            if (Language)
            {
                console_label.Text = "Консоль разработчика";
                nickname_label.Text = "Имя игрока:";
                host_btn.Text = "Создать игру";
                connect_game_btn.Text = "Присоединиться";
                multiplayer_close.Text = "Назад";
                close_host_btn.Text = "Назад";
                start_multiplayer_game.Text = "Играть";
                start_btn.Text = "Начать игру";
                select_mode_btn.Text = "Выбрать";
                close_game_mode_panel.Text = "Закрыть";
                easy_btn.Text = "Легко";
                normal_btn.Text = "Нормально";
                hard_btn.Text = "Сложно";
                very_hard_btn.Text = "Оч. сложно";
                custom_btn.Text = "Редактор";
                close_difficulty_panel.Text = "Назад";
                start_game_btn.Text = "Играть";
                setting_btn.Text = "Настройки";
                volume_label.Text = "Громкость";
                about_developers_btn.Text = "О разработчиках";
                bug_report_btn.Text = "Сообщить об ошибке";
                exit_btn.Text = "Выйти из игры";
                exit_label.Text = "Вы действительно хотите выйти?";
                exit_yes_btn.Text = "Да";
                exit_no_btn.Text = "Нет";
                fatalan_about.Text = "Текстурирование, рендеринг спрайтов, ИИ врагов и питомцев";
                qsvhu_about.Text = "Спрайты и звуки оружия";
                koyo_about.Text = "Текстуры, спрайты врагов и фон меню";
                close_developers.Text = "Закрыть";
                all_settings.Text = "Общие";
                sounds_label.Text = "Игровые звуки";
                language_label.Text = "Язык";
                update_label.Text = "Авто-обновление";
                check_update_btn.Text = "Проверить наличие обновлений";
                video_settings.Text = "Графика";
                high_resolution_label.Text = "Высокое разрешение";
                show_fps_label.Text = "Отображение FPS";
                show_minimap_label.Text = "Отображать мини-карту";
                scope_label.Text = "Прицел: " + GetScopeType();
                scope_color_label.Text = "Цвет прицела: " + GetScopeColor();
                control_settings.Text = "Управление";
                sensitivity_label.Text = "Чувствительность мыши";
                clear_settings.Text = "Сбросить";
                close_settings.Text = "Закрыть";
                change_logs_close_btn.Text = "Закрыть";
                press_any_btn_label.Text = "Нажмите любую кнопку или ESC для отмены";
                screenshot_label.Text = "Скриншот";
                fire_btn.Text = "ЛКМ";
                aim_btn.Text = "ПКМ";
                reloading_label.Text = "Перезарядка";
                fire_label.Text = "Выстрел";
                aim_label.Text = "Прицеливание";
                forward_label.Text = "Вперёд";
                back_label.Text = "Назад";
                left_label.Text = "Влево";
                right_label.Text = "Вправо";
                interaction_label.Text = "Взаимодействие";
                show_map_label.Text = "Показать/скрыть карту";
                flashlight_label.Text = "Фонарик";
                medkit_label.Text = "Использовать предмет";
                select_item_label.Text = "Выбрать предмет";
                run_label.Text = "Бег (удерживать)";
            }
            else
            {
                console_label.Text = "Developer console";
                nickname_label.Text = "Player name:";
                host_btn.Text = "Create game";
                connect_game_btn.Text = "Join";
                multiplayer_close.Text = "Back";
                close_host_btn.Text = "Back";
                start_multiplayer_game.Text = "Play";
                start_btn.Text = "Start game";
                select_mode_btn.Text = "Select";
                close_game_mode_panel.Text = "Close";
                easy_btn.Text = "Easy";
                normal_btn.Text = "Normal";
                hard_btn.Text = "Difficult";
                very_hard_btn.Text = "Very difficult";
                custom_btn.Text = "Editor";
                close_difficulty_panel.Text = "Return";
                start_game_btn.Text = "Play";
                volume_label.Text = "Volume";
                setting_btn.Text = "Settings";
                about_developers_btn.Text = "About developers";
                bug_report_btn.Text = "Report a bug";
                exit_btn.Text = "Exit game";
                exit_label.Text = "Do you really want to leave?";
                exit_yes_btn.Text = "Yes";
                exit_no_btn.Text = "No";
                fatalan_about.Text = "Texturing, sprite rendering, enemy and pet AI";
                qsvhu_about.Text = "Weapon sprites and sounds";
                koyo_about.Text = "Textures, enemy sprites and menu background";
                close_developers.Text = "Close";
                all_settings.Text = "General";
                sounds_label.Text = "Game sounds";
                language_label.Text = "Language";
                update_label.Text = "Auto-update";
                check_update_btn.Text = "Check for update";
                video_settings.Text = "Graphics";
                high_resolution_label.Text = "High resolution";
                show_fps_label.Text = "Display FPS";
                show_minimap_label.Text = "Show minimap";
                scope_label.Text = "Scope: " + GetScopeType();
                scope_color_label.Text = "Scope color: " + GetScopeColor();
                control_settings.Text = "Control";
                sensitivity_label.Text = "Mouse sensitivity";
                clear_settings.Text = "Reset";
                close_settings.Text = "Close";
                change_logs_close_btn.Text = "Close";
                press_any_btn_label.Text = "Press any button or ESC to cancel";
                screenshot_label.Text = "Screenshot";
                fire_btn.Text = "LMB";
                aim_btn.Text = "RMB";
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
            }
            if (ConsoleEnabled)
            {
                if (Language)
                    console_btn.Text = "Вкл.";
                else
                    console_btn.Text = "On";
            }
            else
            {
                if (Language)
                    console_btn.Text = "Откл.";
                else
                    console_btn.Text = "Off";
            }
            if (sounds)
            {
                if (Language)
                    sounds_on_off.Text = "Вкл.";
                else
                    sounds_on_off.Text = "On";
            }
            else
            {
                if (Language)
                    sounds_on_off.Text = "Откл.";
                else
                    sounds_on_off.Text = "Off";
            }
            if (update_on_off.Checked)
            {
                if (Language)
                    update_on_off.Text = "Вкл.";
                else
                    update_on_off.Text = "On";
            }
            else
            {
                if (Language)
                    update_on_off.Text = "Откл.";
                else
                    update_on_off.Text = "Off";
            }
            if (high_resolution_on_off.Checked)
            {
                if (Language)
                    high_resolution_on_off.Text = "Вкл.";
                else
                    high_resolution_on_off.Text = "On";
            }
            else
            {
                if (Language)
                    high_resolution_on_off.Text = "Откл.";
                else
                    high_resolution_on_off.Text = "Off";
            }
            if (ShowFPS)
            {
                if (Language)
                    show_fps_on_off.Text = "Вкл.";
                else
                    show_fps_on_off.Text = "On";
            }
            else
            {
                if (Language)
                    show_fps_on_off.Text = "Откл.";
                else
                    show_fps_on_off.Text = "Off";
            }
            if (ShowMiniMap)
            {
                if (Language)
                    show_minimap.Text = "Вкл.";
                else
                    show_minimap.Text = "On";
            }
            else
            {
                if (Language)
                    show_minimap.Text = "Откл.";
                else
                    show_minimap.Text = "Off";
            }
            difficulty_label.Text = AboutDifficulty[Language ? 0 : 1, difficulty];
            start_btn.Size = new Size(0, 0);
            setting_btn.Size = new Size(0, 0);
            about_developers_btn.Size = new Size(0, 0);
            bug_report_btn.Size = new Size(0, 0);
            exit_btn.Size = new Size(0, 0);
            check_update_btn.Size = new Size(0, 0);
            start_btn.Left = (button_background.Width - start_btn.Width) / 2;
            setting_btn.Left = (button_background.Width - setting_btn.Width) / 2;
            about_developers_btn.Left = (button_background.Width - about_developers_btn.Width) / 2;
            bug_report_btn.Left = (button_background.Width - bug_report_btn.Width) / 2;
            exit_btn.Left = (button_background.Width - exit_btn.Width) / 2;
        }

        private void Start_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            game_mode_panel.Visible = true;
            game_mode_panel.BringToFront();
        }

        private void Sounds_on_off_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            sounds = sounds_on_off.Checked;
            if (sounds)
            {
                if (Language)
                    sounds_on_off.Text = "Вкл.";
                else
                    sounds_on_off.Text = "On";
                MainMenuTheme.Play(Volume);
            }
            else
            {
                if (Language)
                    sounds_on_off.Text = "Откл.";
                else
                    sounds_on_off.Text = "Off";
                MainMenuTheme.Stop();
            }
        }

        private void Console_btn_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            ConsoleEnabled = console_btn.Checked;
            if (ConsoleEnabled)
            {
                if (Language)
                    console_btn.Text = "Вкл.";
                else
                    console_btn.Text = "On";
            }
            else
            {
                if (Language)
                    console_btn.Text = "Откл.";
                else
                    console_btn.Text = "Off";
            }
        }

        private void Language_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            Language = language_list.SelectedIndex == 0;
            SetLanguage();
        }

        private void Setting_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            settings_panel.Visible = true;
            settings_panel.BringToFront();
        }

        private void Update_on_off_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            if (update_on_off.Checked)
            {
                if (Language)
                    update_on_off.Text = "Вкл.";
                else
                    update_on_off.Text = "On";
            }
            else
            {
                if (Language)
                    update_on_off.Text = "Откл.";
                else
                    update_on_off.Text = "Off";
            }
        }

        private void Check_update_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            Check_Update(false);
        }

        private void High_resolution_on_off_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            resolution = high_resolution_on_off.Checked ? 1 : 0;
            if (high_resolution_on_off.Checked)
            {
                if (Language)
                    high_resolution_on_off.Text = "Вкл.";
                else
                    high_resolution_on_off.Text = "On";
            }
            else
            {
                if (Language)
                    high_resolution_on_off.Text = "Откл.";
                else
                    high_resolution_on_off.Text = "Off";
            }
        }

        private void Fps_Scroll(object sender, EventArgs e)
        {
            hight_fps = fps.Value == 1;
            if (hight_fps)
                fps_label.Text = "FPS: 60";
            else
                fps_label.Text = "FPS: 30";
        }

        private string GetScopeType()
        {
            switch (scope_type)
            {
                case 0:
                    if (Language)
                        return "Стандартный";
                    return "Standard";
                case 1:
                    if (Language)
                        return "Крест";
                    return "Cross";
                case 2:
                    if (Language)
                        return "Линия";
                    return "Line";
                case 3:
                    if (Language)
                        return "Точка";
                    return "Dot";
                default:
                    if (Language)
                        return "Без прицела";
                    return "No scope";
            }
        }

        private string GetScopeColor()
        {
            switch (scope_color)
            {
                case 0:
                    if (Language)
                        return "Зелёный";
                    return "Green";
                case 1:
                    if (Language)
                        return "Красный";
                    return "Red";
                case 2:
                    if (Language)
                        return "Жёлтый";
                    return "Yellow";
                case 3:
                    if (Language)
                        return "Синий";
                    return "Blue";
                case 4:
                    if (Language)
                        return "Пурпурный";
                    return "Purple";
                case 5:
                    if (Language)
                        return "Голубой";
                    return "Cyan";
                case 6:
                    if (Language)
                        return "Оранжевый";
                    return "Orange";
                case 7:
                    if (Language)
                        return "Белый";
                    return "White";
                default:
                    if (Language)
                        return "Случайный";
                    return "Random";
            }
        }

        private void Sensitivity_Scroll(object sender, EventArgs e) => LOOK_SPEED = (double)sensitivity.Value / 100;

        private void Show_minimap_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            ShowMiniMap = show_minimap.Checked;
            if (ShowMiniMap)
            {
                if (Language)
                    show_minimap.Text = "Вкл.";
                else
                    show_minimap.Text = "On";
            }
            else
            {
                if (Language)
                    show_minimap.Text = "Откл.";
                else
                    show_minimap.Text = "Off";
            }
        }

        private void Show_fps_on_off_CheckedChanged(object sender, EventArgs e)
        {
            lose_focus.Focus();
            ShowFPS = show_fps_on_off.Checked;
            if (ShowFPS)
            {
                if (Language)
                    show_fps_on_off.Text = "Вкл.";
                else
                    show_fps_on_off.Text = "On";
            }
            else
            {
                if (Language)
                    show_fps_on_off.Text = "Откл.";
                else
                    show_fps_on_off.Text = "Off";
            }
        }

        private void Clear_settings_Click(object sender, EventArgs e)
        {
            Language = Check_Language();
            sounds = true;
            ConsoleEnabled = false;
            update_on_off.Checked = true;
            resolution = 0;
            ShowFPS = true;
            hight_fps = true;
            ShowMiniMap = true;
            scope_type = 0;
            scope_color = 0;
            LOOK_SPEED = 6.5;
            Volume = 0.4f;
            BindControls.Clear();
            foreach (var kvp in ClassicBindControls)
                BindControls.Add(kvp.Key, kvp.Value);
            SetVisualSettings();
        }

        private void SaveSettings()
        {
            INIReader.SetKey(iniFolder, "CONFIG", "sounds", sounds);
            INIReader.SetKey(iniFolder, "CONFIG", "language", Language);
            INIReader.SetKey(iniFolder, "CONFIG", "console_enabled", ConsoleEnabled);
            INIReader.SetKey(iniFolder, "CONFIG", "auto_update", update_on_off.Checked);
            INIReader.SetKey(iniFolder, "SLIL", "hight_resolution", high_resolution_on_off.Checked);
            INIReader.SetKey(iniFolder, "SLIL", "show_fps", ShowFPS);
            INIReader.SetKey(iniFolder, "SLIL", "hight_fps", hight_fps);
            INIReader.SetKey(iniFolder, "SLIL", "show_minimap", ShowMiniMap);
            INIReader.SetKey(iniFolder, "SLIL", "scope_type", scope_type);
            INIReader.SetKey(iniFolder, "SLIL", "scope_color", scope_color);
            INIReader.SetKey(iniFolder, "SLIL", "look_speed", LOOK_SPEED);
            INIReader.SetKey(iniFolder, "SLIL", "volume", Volume);
            INIReader.SetKey(iniFolder, "SLIL", "screenshot", BindControls["screenshot"]);
            INIReader.SetKey(iniFolder, "SLIL", "reloading", BindControls["reloading"]);
            INIReader.SetKey(iniFolder, "SLIL", "forward", BindControls["forward"]);
            INIReader.SetKey(iniFolder, "SLIL", "back", BindControls["back"]);
            INIReader.SetKey(iniFolder, "SLIL", "left", BindControls["left"]);
            INIReader.SetKey(iniFolder, "SLIL", "right", BindControls["right"]);
            INIReader.SetKey(iniFolder, "SLIL", "interaction_0", BindControls["interaction_0"]);
            INIReader.SetKey(iniFolder, "SLIL", "interaction_1", BindControls["interaction_1"]);
            INIReader.SetKey(iniFolder, "SLIL", "show_map_0", BindControls["show_map_0"]);
            INIReader.SetKey(iniFolder, "SLIL", "show_map_1", BindControls["show_map_1"]);
            INIReader.SetKey(iniFolder, "SLIL", "flashlight", BindControls["flashlight"]);
            INIReader.SetKey(iniFolder, "SLIL", "item", BindControls["item"]);
            INIReader.SetKey(iniFolder, "SLIL", "select_item", BindControls["select_item"]);
            INIReader.SetKey(iniFolder, "SLIL", "run", BindControls["run"]);
        }

        private void Difficulty_CheckedChanged(object sender, EventArgs e)
        {
            if (custom_btn.Checked)
            {
                difficulty = 4;
                start_game_btn.Text = Language ? "Редактор" : "Editor";
            }
            else
            {
                start_game_btn.Text = Language ? "Играть" : "Play";
                if (easy_btn.Checked)
                    difficulty = 3;
                else if (normal_btn.Checked)
                    difficulty = 2;
                else if (hard_btn.Checked)
                    difficulty = 1;
                else
                    difficulty = 0;
            }
            difficulty_label.Text = AboutDifficulty[Language ? 0 : 1, difficulty];
        }

        private void Close_difficulty_panel_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            difficulty_panel.Visible = false;
        }

        private void Start_game_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            game_mode_panel.Visible = false;
            if (difficulty != 4)
            {
                if(sounds) MainMenuTheme.Stop();
                SLIL form = new SLIL(textureCache)
                {
                    ost = ost,
                    //PETS = PETS,
                    step = step,
                    DeathSounds = DeathSounds,
                    CuteDeathSounds = CuteDeathSounds,
                    game_over = game_over,
                    draw = draw,
                    buy = buy,
                    hit = hit,
                    hungry = hungry,
                    wall = wall,
                    tp = tp,
                    screenshot = screenshot,
                    door = door,
                };
                form.ShowDialog();
                if (sounds) MainMenuTheme.Play(Volume);
                difficulty_panel.Visible = false;
            }
            else
            {
                Editor = new SLIL_Editor
                {
                    Owner = this,
                    MazeHeight = 13,
                    MazeWidth = 13
                };
                Editor.FormClosing += EditorForm_FormClosing;
                Editor.ShowDialog();
            }
        }

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
            changes_list.Items.Clear();
            if (Language)
                changes_list.Items.AddRange(ru_changes);
            else
                changes_list.Items.AddRange(en_changes);
        }

        private void Change_logs_close_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            change_logs_panel.Visible = false;
        }

        private void Close_game_mode__panel_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            game_mode_panel.Visible = false;
        }

        private void Select_mode_btn_Click(object sender, EventArgs e)
        {
            if (singleplayer.Checked)
            {
                difficulty_panel.Visible = true;
                difficulty_panel.BringToFront();
            }
            else
            {
                multiplayer_panel.Visible = true;
                multiplayer_panel.BringToFront();
            }
        }

        private void Volume_Scroll(object sender, EventArgs e)
        {
            Volume = (float)volume.Value / 100;
            MainMenuTheme.SetVolume(Volume);
        }

        private void Copy_ip_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            Clipboard.SetText(ip.Text);
            if (Language)
                MessageBox.Show("IP-адрес успешно скопирован в буфер обмена", "Копирование IP", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("IP address successfully copied to clipboard", "IP Copying", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Host_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            host_panel.Visible = true;
            host_panel.BringToFront();
        }

        private void Close_host_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            host_panel.Visible = false;
            players_panel.Controls.Clear();
        }

        private void Start_multiplayer_game_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();

        }

        private void Connect_game_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            ip_connect_input.Text = "000.000.000.000:0000";
            connect_panel.Visible = true;
            connect_panel.BringToFront();
        }

        private void Close_connect_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            connect_panel.Visible = false;
        }

        private void Connect_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            game_mode_panel.Visible = false;
            SLIL form = new SLIL(textureCache, ip_connect_input.Text.Split(':')[0], int.Parse(ip_connect_input.Text.Split(':')[1]));
            form.ShowDialog();
            difficulty_panel.Visible = false;
        }

        private void Multiplayer_close_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            multiplayer_panel.Visible = false;
        }

        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Editor != null && Editor.OK)
            {
                if (sounds) MainMenuTheme.Stop();
                SLIL form = new SLIL(textureCache, true, Editor.MAP, (Editor.MazeWidth - 1) / 3,(Editor.MazeHeight - 1) / 3, SLIL_Editor.x, SLIL_Editor.y)
                {
                    ost = ost,
                    //PETS = PETS,
                    step = step,
                    DeathSounds = DeathSounds,
                    CuteDeathSounds = CuteDeathSounds,
                    game_over = game_over,
                    draw = draw,
                    buy = buy,
                    hit = hit,
                    hungry = hungry,
                    wall = wall,
                    tp = tp,
                    screenshot = screenshot,
                    door = door,
                };
                form.ShowDialog();
                if (sounds) MainMenuTheme.Play(Volume);
                Editor = null;
                difficulty_panel.Visible = false;
            }
        }

        private void Scope_color_choice_Scroll(object sender, EventArgs e)
        {
            scope_color = scope_color_choice.Value;
            if (Language)
                scope_color_label.Text = "Цвет прицела: " + GetScopeColor();
            else
                scope_color_label.Text = "Scope color: " + GetScopeColor();
        }

        private void Scope_choice_Scroll(object sender, EventArgs e)
        {
            scope_type = scope_choice.Value;
            if (Language)
                scope_label.Text = "Прицел: " + GetScopeType();
            else
                scope_label.Text = "Scope: " + GetScopeType();
        }

        private void About_developers_btn_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            developers_panel.Visible = true;
            developers_panel.BringToFront();
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
                if (BindControls.ContainsValue(key) || key == Keys.Oemtilde || key.ToString().StartsWith("Num") || (key.ToString().StartsWith("D") && key.ToString().Length == 2))
                    return;
                if (BindControls.ContainsKey(SelectButtonName))
                {
                    BindControls[SelectButtonName] = key;
                    Button btn = Controls.Find(SelectButtonName + "_btn", true)[0] as Button;
                    btn.Text = key.ToString().Replace("Key", null).Replace("Return", "Enter");
                    ChangeControlButton = false;
                    press_any_btn_panel.Visible = false;
                    INIReader.SetKey(iniFolder, "SLIL", SelectButtonName, key);
                }
            }
        }

        private void ChangeControl_Click(object sender, EventArgs e)
        {
            lose_focus.Focus();
            if (!ChangeControlButton)
            {
                ChangeControlButton = true;
                SelectButtonName = (sender as Button).Name.Replace("_btn", null);
                press_any_btn_panel.Visible = true;
                press_any_btn_panel.BringToFront();
            }
        }
    }
}