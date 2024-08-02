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

namespace SLIL
{
    public partial class MainMenu : Form
    {
        private const string current_version = "|1.0.1|";
        public static string iniFolder = "config.ini";
        public static bool Language = true, sounds = true;
        private readonly TextureCache textureCache = new TextureCache();
        public static CGF_Reader CGFReader;
        private string SelectButtonName;
        private SLIL_Editor Editor;
        private bool ChangeControlButton = false;
        private readonly string[,] AboutDifficulty =
        {
            {
                // Очень сложно
                "Начальное оружие: Пистолет 1 ур.\nВраги возрождаются каждые 60 секунд\nВраги более агрессивны",
                // Сложно
                "Начальное оружие: Пистолет 1 ур.\nВраги возрождаются каждые 60 секунд",
                // Нормально
                "Начальное оружие: Пистолет 2 ур.",
                // Легко
                "Начальное оружие: Пистолет 2 ур.\nВраги дают больше денег",
                // Редактор
                "Параметры уровня могут быть изменены вручную"
            },
            {
                // Очень сложно
                "Starting weapon: Pistol Lv1\nEnemies respawn every 60 seconds\nEnemies are more aggressive",
                // Сложно
                "Starting weapon: Pistol Lv1\nEnemies respawn every 60 seconds",
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
            { "medkit", Keys.H },
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
            { "medkit", Keys.H },
            { "run", Keys.ShiftKey },
        };
        public static int resolution = 0, scope_type = 0, scope_color = 0, difficulty = 2;
        public static bool hight_fps = true, ShowFPS = true, ShowMiniMap = true;
        public static double LOOK_SPEED = 6.5;

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
        }

        private void Bug_report_btn_Click(object sender, EventArgs e)
        {
            by.Focus();
            Process.Start(new ProcessStartInfo("https://t.me/MiniGamesBugReport_BOT") { UseShellExecute = true });
        }

        private void Developer_name_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Process.Start(new ProcessStartInfo("https://github.com/Lonewolf239") { UseShellExecute = true });
        }

        private void Web_site_lonewolf_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://base-escape.ru") { UseShellExecute = true });

        private void Telegram_lonewolf_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://t.me/+VLJzjVRg8ElkZWYy") { UseShellExecute = true });

        private void Github_lonewolf_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/Lonewolf239") { UseShellExecute = true });

        private void Fatalan_git_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/Fatalan") { UseShellExecute = true });

        private void Qsvhu_telegram_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://t.me/Apsyuch") { UseShellExecute = true });

        private void Koyo_hipolink_Click(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://hipolink.me/koyomichu") { UseShellExecute = true });

        private void Close_developers_Click(object sender, EventArgs e) => developers_panel.Visible = false;

        private void MainMenu_FormClosing(object sender, FormClosingEventArgs e) => SaveSettings();

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
            by.Focus();
            exit_panel.Visible = false;
        }

        private void Exit_yes_btn_Click(object sender, EventArgs e) => Application.Exit();

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
            if (Directory.Exists("sounds"))
                Directory.Delete("sounds", true);
            version_label.Text = $"v{current_version.Replace("|", "")}";
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
            exit_size_panel.Left = (exit_panel.Width - exit_size_panel.Width) / 2;
        }

        private void GetGameParametrs()
        {
            Language = INIReader.GetBool(iniFolder, "CONFIG", "language", Language);
            sounds = INIReader.GetBool(iniFolder, "CONFIG", "sounds", true);
            update_on_off.Checked = INIReader.GetBool(iniFolder, "CONFIG", "auto_update", true);
            LOOK_SPEED = INIReader.GetDouble(iniFolder, "SLIL", "look_speed", 2.5);
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
            BindControls["medkit"] = INIReader.GetKeys(iniFolder, "SLIL", "medkit", Keys.H);
            BindControls["run"] = INIReader.GetKeys(iniFolder, "SLIL", "run", Keys.ShiftKey);
            if (LOOK_SPEED < 2.5 || LOOK_SPEED > 10)
                LOOK_SPEED = 6.5;
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
            medkit_btn.Text = BindControls["medkit"].ToString().Replace("Key", null).Replace("Return", "Enter");
            run_btn.Text = BindControls["run"].ToString().Replace("Key", null).Replace("Return", "Enter");
            language_list.SelectedIndex = Language ? 0 : 1;
            sounds_on_off.Checked = sounds;
            high_resolution_on_off.Checked = resolution == 1;
            show_fps_on_off.Checked = ShowFPS;
            fps.Value = hight_fps ? 1 : 0;
            show_minimap.Checked = ShowMiniMap;
            scope_choice.Value = scope_type;
            scope_color_choice.Value = scope_color;
            sensitivity.Value = (int)(LOOK_SPEED * 100);
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
                start_btn.Text = "Начать игру";
                easy_btn.Text = "Легко";
                normal_btn.Text = "Нормально";
                hard_btn.Text = "Сложно";
                very_hard_btn.Text = "Оч. сложно";
                custom_btn.Text = "Редактор";
                close_difficulty_panel.Text = "Закрыть";
                start_game_btn.Text = "Играть";
                setting_btn.Text = "Настройки";
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
                medkit_label.Text = "Использовать аптечку";
                run_label.Text = "Бег (удерживать)";
            }
            else
            {
                start_btn.Text = "Start game";
                easy_btn.Text = "Easy";
                normal_btn.Text = "Normal";
                hard_btn.Text = "Difficult";
                very_hard_btn.Text = "Very difficult";
                custom_btn.Text = "Editor";
                close_difficulty_panel.Text = "Close";
                start_game_btn.Text = "Play";
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
                medkit_label.Text = "Use medkit";
                run_label.Text = "Run (hold)";
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
            by.Focus();
            difficulty_panel.Visible = true;
            difficulty_panel.BringToFront();
        }

        private void Sounds_on_off_CheckedChanged(object sender, EventArgs e)
        {
            by.Focus();
            sounds = sounds_on_off.Checked;
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
        }

        private void Language_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            by.Focus();
            Language = language_list.SelectedIndex == 0;
            SetLanguage();
        }

        private void Setting_btn_Click(object sender, EventArgs e)
        {
            by.Focus();
            settings_panel.Visible = true;
            settings_panel.BringToFront();
        }

        private void Update_on_off_CheckedChanged(object sender, EventArgs e)
        {
            by.Focus();
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
            by.Focus();
            Check_Update(false);
        }

        private void High_resolution_on_off_CheckedChanged(object sender, EventArgs e)
        {
            by.Focus();
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

        private void Sensitivity_Scroll(object sender, EventArgs e) => LOOK_SPEED = sensitivity.Value / 100;

        private void Show_minimap_CheckedChanged(object sender, EventArgs e)
        {
            by.Focus();
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
            by.Focus();
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
            update_on_off.Checked = true;
            resolution = 0;
            ShowFPS = true;
            hight_fps = true;
            ShowMiniMap = true;
            scope_type = 0;
            scope_color = 0;
            LOOK_SPEED = 6.5;
            BindControls.Clear();
            foreach (var kvp in ClassicBindControls)
                BindControls.Add(kvp.Key, kvp.Value);
            SetVisualSettings();
        }

        private void SaveSettings()
        {
            INIReader.SetKey(iniFolder, "CONFIG", "sounds", sounds);
            INIReader.SetKey(iniFolder, "CONFIG", "language", Language);
            INIReader.SetKey(iniFolder, "CONFIG", "auto_update", update_on_off.Checked);
            INIReader.SetKey(iniFolder, "SLIL", "hight_resolution", high_resolution_on_off.Checked);
            INIReader.SetKey(iniFolder, "SLIL", "show_fps", ShowFPS);
            INIReader.SetKey(iniFolder, "SLIL", "hight_fps", hight_fps);
            INIReader.SetKey(iniFolder, "SLIL", "show_minimap", ShowMiniMap);
            INIReader.SetKey(iniFolder, "SLIL", "scope_type", scope_type);
            INIReader.SetKey(iniFolder, "SLIL", "scope_color", scope_color);
            INIReader.SetKey(iniFolder, "SLIL", "look_speed", LOOK_SPEED);
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
            INIReader.SetKey(iniFolder, "SLIL", "medkit", BindControls["medkit"]);
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
            by.Focus();
            difficulty_panel.Visible = false;
        }

        private void Start_game_btn_Click(object sender, EventArgs e)
        {
            by.Focus();
            if (difficulty != 4)
            {
                SLIL form = new SLIL(textureCache);
                form.ShowDialog();
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

        private void EditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Editor != null && Editor.OK)
            {
                SLIL form = new SLIL(textureCache)
                {
                    CUSTOM = true,
                    CUSTOM_MAP = Editor.MAP,
                    CustomMazeHeight = (Editor.MazeHeight - 1) / 3,
                    CustomMazeWidth = (Editor.MazeWidth - 1) / 3,
                    CUSTOM_X = SLIL_Editor.x,
                    CUSTOM_Y = SLIL_Editor.y
                };
                form.ShowDialog();
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
            by.Focus();
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
            by.Focus();
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