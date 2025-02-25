using System;
using System.IO;
using CGFReader;
using SLIL.Classes;
using System.Drawing;
using System.Net.Http;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using SLIL.SLIL_Localization;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace SLIL
{
    internal partial class Loading : Form
    {
        private readonly string current_version = Program.current_version;
        private double UpdateProgress = 0, CompletedProgress = 0;
        private bool UpdateVerified = false, CurrentVersion = false;
        private bool DownloadedLocalizationList = false;
        internal string HashedKey = "None", License = "None";
        private LoginForm loginForm;
        internal static Localization Localizations = new Localization();
        internal SLILAccount SLIL_Account;
        private TextureCache textureCache;
        private CGF_Reader CGFReader;
        private MainMenu mainMenu;
        private readonly Dictionary<string, string> SupportedLanguages = new Dictionary<string, string>();
        private bool isDragging = false;
        private Point lastCursor, lastForm;

        internal Loading()
        {
            InitializeComponent();
            Cursor = Program.SLILCursorDefault;
        }

        private static bool CheckInternet()
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send("8.8.8.8", 2000);
                    return (reply != null && reply.Status == IPStatus.Success);
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task SetLocalizations(string[] codes, string[] languages)
        {
            status_label.Text = "Unpacking localization...";
            SupportedLanguages.Clear();
            for (int i = 0; i < languages.Length; i++)
                SupportedLanguages.Add(codes[i], languages[i]);
            for (int i = 0; i < languages.Length; i++)
            {
                try
                {
                    await Localizations.DownloadLocalization(languages[i]);
                }
                catch
                {
                    DownloadedLocalizationList = false;
                    return;
                }
            }
            DownloadedLocalizationList = true;
        }

        private async Task DownloadLocalizationListTask()
        {
            double startProgress = UpdateProgress;
            var progress = new Progress<int>(percent =>
            {
                if (percent == 100) UpdateProgress = startProgress + 5;
                else UpdateProgress = startProgress + (percent * 10 / 100);
            });
            await DownloadLocalizationList(progress);
        }

        private async Task DownloadLocalizationList(IProgress<int> progress)
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
                        int totalProgressSteps = lines.Length + 1;
                        for (int i = 0; i < lines.Length; i++)
                        {
                            progress.Report((i + 1) * 100 / totalProgressSteps);
                            codes[i] = lines[i].Split('-')[0];
                            languages[i] = lines[i].Split('-')[1];
                        }
                        progress.Report(100);
                        await SetLocalizations(codes, languages);
                    }
                }
                catch
                {
                    DownloadedLocalizationList = false;
                    return;
                }
            }
            Localizations.RemoveDuplicates();
        }

        private static async Task DownloadFileAsync(string url, string outputPath)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    using (var response = await httpClient.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode();
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                                await stream.CopyToAsync(fileStream);
                        }
                    }
                }
                catch { }
            }
        }

        private async Task Check_Update()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    string content = await httpClient.GetStringAsync("https://base-escape.ru/version_SLIL.txt");
                    string line = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
                    if (!line.Contains(current_version))
                    {
                        if (!File.Exists("UpdateDownloader.exe"))
                            await DownloadFileAsync("https://base-escape.ru/downloads/UpdateDownloader.exe", "UpdateDownloader.exe");
                    }
                    else CurrentVersion = true;
                    UpdateVerified = true;
                }
            }
            catch
            {
                CurrentVersion = true;
                UpdateVerified = false;
            }
        }

        internal async Task ProcessFileWithProgressAsync()
        {
            double startProgress = UpdateProgress;
            var progress = new Progress<int>(percent =>
            {
                if (percent == 100) UpdateProgress = startProgress + 35;
                else UpdateProgress = startProgress + (percent * 35 / 100);
            });
            await CGFReader.ProcessFileAsync(progress);
        }

        internal async Task ProcessTexturesWithProgressAsync()
        {
            double startProgress = UpdateProgress;
            var progress = new Progress<int>(percent =>
            {
                if (percent == 100)
                    UpdateProgress = startProgress + 35;
                else
                    UpdateProgress = startProgress + (percent * 35 / 100);
            });
            await textureCache.LoadTextures(progress);
        }

        private async Task LoadingMainMenu()
        {
            if (!CheckInternet())
            {
                UpdateProgress = 15;
                await UnpackData();
            }
            else
                await CheckUpdate();
        }

        private async Task CheckUpdate()
        {
            await Check_Update();
            if (!CurrentVersion)
            {
                Process.Start(new ProcessStartInfo("UpdateDownloader.exe", "https://base-escape.ru/downloads/Setup_SLIL.exe Setup_SLIL"));
                Application.Exit();
                return;
            }
            UpdateProgress = 5;
            await DownloadLocalization();
        }

        private async Task DownloadLocalization()
        {
            status_label.Text = "Downloading localization...";
            await DownloadLocalizationListTask();
            UpdateProgress += 5;
            await UnpackData();
        }

        private async Task UnpackData()
        {
            status_label.Text = "Unpacking \"data.cgf\" file...";
            if (File.Exists("data.cgf"))
            {
                CGFReader = new CGF_Reader("data.cgf");
                await ProcessFileWithProgressAsync();
                await TextureCache();
            }
            else
            {
                string title = "Missing \"data.cgf\" file!", message = $"The file \"data.cgf\" is missing! It may have been renamed, moved, or deleted. Do you want to download the installer again?";
                if (DownloadedLocalizationList)
                {
                    title = Localizations.GetLString(Program.iniReader.GetString("CONFIG", "language", "English"), "0-92");
                    message = Localizations.GetLString(Program.iniReader.GetString("CONFIG", "language", "English"), "0-93");
                }
                if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                {
                    if (!File.Exists("UpdateDownloader.exe"))
                    {
                        message = "UpdateDownloader.exe has been deleted, renamed, or moved. After closing this message, it will be downloaded again.";
                        string caption = "Error";
                        if (DownloadedLocalizationList)
                        {
                            caption = Localizations.GetLString(Program.iniReader.GetString("CONFIG", "language", "English"), "0-94");
                            message = Localizations.GetLString(Program.iniReader.GetString("CONFIG", "language", "English"), "0-95");
                        }
                        MessageBox.Show(message, caption, MessageBoxButtons.OK);
                        await DownloadFileAsync("https://base-escape.ru/downloads/UpdateDownloader.exe", "UpdateDownloader.exe");
                    }
                    Process.Start(new ProcessStartInfo("UpdateDownloader.exe", "https://base-escape.ru/downloads/Setup_SLIL.exe Setup_SLIL"));
                    Application.Exit();
                    return;
                }
                else
                {
                    Application.Exit();
                    return;
                }
            }
        }

        private async Task TextureCache()
        {
            status_label.Text = "Texture caching...";
            textureCache = new TextureCache();
            await ProcessTexturesWithProgressAsync();
            await Login();
        }

        private async Task Login()
        {
            status_label.Text = "Login...";
            HashedKey = Program.iniReader.GetString("ACCOUNT", "key", "None");
            if (!CheckInternet())
            {
                License = Program.iniReader.GetString("ACCOUNT", "license", "None");
                if (Hasher.CheckLicense(License, HashedKey))
                {
                    if (loginForm != null)
                    {
                        loginForm.CanClose = true;
                        loginForm.Close();
                        loginForm.Dispose();
                        loginForm = null;
                    }
                    GoToMainMenu();
                }
                else
                {
                    loginForm = new LoginForm()
                    {
                        DownloadedLocalizationList = DownloadedLocalizationList,
                        SupportedLanguages = SupportedLanguages
                    };
                    loginForm.exit_btn_cp.Click += Exit_btn_cp_Click;
                    loginForm.error_panel.Visible = true;
                    loginForm.login_panel.Visible = false;
                    loginForm.Error = -3;
                    Hide();
                    loginForm.ShowDialog();
                    Show();
                }
                return;
            }
            if (HashedKey == "None")
            {
                loginForm = new LoginForm()
                {
                    DownloadedLocalizationList = DownloadedLocalizationList,
                    SupportedLanguages = SupportedLanguages
                };
                loginForm.login_btn_r.Click += Login_btn_Click;
                loginForm.buy_btn_cp.Click += Buy_btn_cp_Click;
                loginForm.exit_btn_cp.Click += Exit_btn_cp_Click;
                Hide();
                loginForm.ShowDialog();
                Show();
                return;
            }
            await CheckBD();
        }

        private void Exit_btn_cp_Click(object sender, EventArgs e)
        {
            if (loginForm != null)
            {
                loginForm.CanClose = true;
                loginForm.Close();
                loginForm.Dispose();
                loginForm = null;
            }
            Program.iniReader.RemoveKey("ACCOUNT", "license");
            Application.Exit();
        }

        private void Buy_btn_cp_Click(object sender, EventArgs e)
        {
            if (loginForm != null)
            {
                loginForm.CanClose = true;
                loginForm.Close();
                loginForm.Dispose();
                loginForm = null; 
            }
            Process.Start(new ProcessStartInfo("https://t.me/SLIL_AccountBOT") { UseShellExecute = true });
            Application.Exit();
        }

        private async void Login_btn_Click(object sender, EventArgs e)
        {
            HashedKey = Hasher.HashSha256(loginForm.key_input.Text);
            await CheckBD();
        }

        internal async Task CheckBD()
        {
            SLIL_Account = new SLILAccount(HashedKey);
            AccountStates state = await SLIL_Account.LoadAccount();
            if (state == AccountStates.AllOk)
            {
                Program.iniReader.SetKey("ACCOUNT", "key", HashedKey);
                if (!SLIL_Account.AllOk)
                {
                    Program.iniReader.SetKey("ACCOUNT", "license", "None");
                    if (loginForm == null)
                    {
                        loginForm = new LoginForm()
                        {
                            DownloadedLocalizationList = DownloadedLocalizationList,
                            SupportedLanguages = SupportedLanguages
                        };
                    }
                    loginForm.buy_btn_cp.Click += Buy_btn_cp_Click;
                    loginForm.buy_panel.Visible = true;
                    loginForm.login_panel.Visible = false;
                    if (!loginForm.Visible)
                    {
                        Hide();
                        loginForm.ShowDialog();
                        Show();
                    }
                }
                else
                {
                    if (loginForm != null)
                    {
                        loginForm.CanClose = true;
                        loginForm.Close();
                        loginForm.Dispose();
                        loginForm = null;
                    }
                    License = Hasher.SetLicense(HashedKey);
                    Program.iniReader.SetKey("ACCOUNT", "license", License);
                    GoToMainMenu();
                }
            }
            else
            {
                if (state == AccountStates.NotFound || state == AccountStates.AlreadyUsed)
                {
                    if (loginForm == null)
                    {
                        loginForm = new LoginForm()
                        {
                            DownloadedLocalizationList = DownloadedLocalizationList,
                            SupportedLanguages = SupportedLanguages
                        };
                    }
                    loginForm.login_btn_r.Click += Login_btn_Click;
                    loginForm.exit_btn_cp.Click += Exit_btn_cp_Click;
                    loginForm.status_label.Visible = true;
                    loginForm.key_input.Text = null;
                    if (!loginForm.Visible)
                    {
                        Hide();
                        loginForm.ShowDialog();
                        Show();
                    }
                }
                else
                {
                    if (loginForm == null)
                    {
                        loginForm = new LoginForm()
                        {
                            DownloadedLocalizationList = DownloadedLocalizationList,
                            SupportedLanguages = SupportedLanguages
                        };
                    }
                    loginForm.exit_btn_cp.Click += Exit_btn_cp_Click;
                    loginForm.error_panel.Visible = true;
                    loginForm.login_panel.Visible = false;
                    if (state == AccountStates.Error)
                        loginForm.Error = -1;
                    else if (state == AccountStates.ErrorDownloading)
                        loginForm.Error = -2;
                    if (!loginForm.Visible)
                    {
                        Hide();
                        loginForm.ShowDialog();
                        Show();
                    }
                }
            }
            loginForm?.SetLanguage();
        }

        private async Task SLILInitialization()
        {
            status_label.Text = "SLIL initialization...";
            await Task.Run(() =>
            {
                SLIL form = new SLIL();
                form.Dispose();
            });
            progress_refresh.Stop();
            mainMenu.Show();
            Hide();
        }

        private async void GoToMainMenu()
        {
            UpdateProgress += 7.5;
            status_label.Text = "Loading main menu...";
            mainMenu = await CreateMainMenuAsync(this);
            mainMenu.FormClosing += MainMenu_FormCLosing;
            UpdateProgress += 7.5;
            await SLILInitialization();
        }

        internal static async Task<MainMenu> CreateMainMenuAsync(Loading loading)
        {
            return await Task.Run(() =>
            {
                MainMenu mainMenu = new MainMenu(loading.CGFReader, loading.textureCache)
                {
                    UpdateVerified = loading.UpdateVerified,
                    downloadedLocalizationList = loading.DownloadedLocalizationList,
                    localizations = Localizations,
                    supportedLanguages = loading.SupportedLanguages,
                    HashedKey = loading.HashedKey,
                    License = loading.License
                };
                return mainMenu;
            });
        }

        private void Progress_refresh_Tick(object sender, EventArgs e)
        {
            if (CompletedProgress < UpdateProgress)
                CompletedProgress++;
            progress_label.Text = $"{CompletedProgress}%";
            progress.Width = (int)(CompletedProgress / 100 * background_progress.Width);
            if (progress.Width == background_progress.Width)
                progress_refresh.Stop();
        }

        private async void Loading_Load(object sender, EventArgs e)
        {
            if (!File.Exists("data.enc")) await DownloadFileAsync(@"https://base-escape.ru/downloads/SLIL/data.enc", "data.enc");
            await LoadingMainMenu();
        }

        private void Close_btn_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Application.Exit();
        }

        private void Close_btn_MouseEnter(object sender, EventArgs e) => close_btn.Image = Properties.Resources.close_entered;

        private void Close_btn_MouseLeave(object sender, EventArgs e) => close_btn.Image = Properties.Resources.close;

        private void MainMenu_FormCLosing(object sender, FormClosingEventArgs e) => Application.Exit();

        private void Loading_FormClosing(object sender, FormClosingEventArgs e) => progress_refresh.Stop();

        private void Loading_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursor = Cursor.Position;
                lastForm = Location;
            }
        }

        private void Loading_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point delta = Point.Subtract(Cursor.Position, new Size(lastCursor));
                Location = Point.Add(lastForm, new Size(delta));
            }
        }

        private void Loading_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) isDragging = false;
        }
    }
}