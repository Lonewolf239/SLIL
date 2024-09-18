using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using CGFReader;
using IniReader;
using SLIL.Classes;
using SLIL.SLIL_Localization;

namespace SLIL
{
    public partial class Loading : Form
    {
        private readonly string current_version = Program.current_version;
        private int Stage = 0;
        private double UpdateProgress = 0, CompletedProgress = 0;
        private bool UpdateVerified = false, CurrentVersion = false;
        private bool DownloadedLocalizationList = false;
        public static Localization Localizations = new Localization();
        private TextureCache textureCache;
        private CGF_Reader CGFReader;
        private MainMenu mainMenu;
        private readonly Dictionary<string, string> SupportedLanguages = new Dictionary<string, string>();

        public Loading() => InitializeComponent();

        private bool CheckInternet()
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
                if (percent == 100)
                    UpdateProgress = startProgress + 10;
                else
                    UpdateProgress = startProgress + (percent * 10 / 100);
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

        private async Task DownloadFileAsync(string url, string outputPath)
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

        public async Task ProcessFileWithProgressAsync()
        {
            double startProgress = UpdateProgress;
            var progress = new Progress<int>(percent =>
            {
                if (percent == 100)
                    UpdateProgress = startProgress + 40;
                else
                    UpdateProgress = startProgress + (percent * 40 / 100);
            });
            await CGFReader.ProcessFileAsync(progress);
        }

        public async Task ProcessTexturesWithProgressAsync()
        {
            double startProgress = UpdateProgress;
            var progress = new Progress<int>(percent =>
            {
                if (percent == 100)
                    UpdateProgress = startProgress + 40;
                else
                    UpdateProgress = startProgress + (percent * 40 / 100);
            });
            await textureCache.LoadTextures(progress);
        }

        private async Task LoadingMainMenu()
        {
            try
            {
                if (!CheckInternet())
                {
                    Stage = 2;
                    UpdateProgress = 20;
                }
                if (Stage == 0)
                {
                    await Check_Update();
                    if (!CurrentVersion)
                    {
                        Process.Start(new ProcessStartInfo("UpdateDownloader.exe", "https://base-escape.ru/downloads/Setup_SLIL.exe Setup_SLIL"));
                        Application.Exit();
                        return;
                    }
                    Stage++;
                    UpdateProgress = 10;
                }
                if (Stage == 1)
                {
                    status_label.Text = "Downloading localization...";
                    await DownloadLocalizationListTask();
                    Stage++;
                }
                if (Stage == 2)
                {
                    status_label.Text = "Unpacking \"data.cgf\" file...";
                    if (File.Exists("data.cgf"))
                    {
                        CGFReader = new CGF_Reader("data.cgf");
                        await ProcessFileWithProgressAsync();
                        Stage++;
                    }
                    else
                    {
                        string title = "Missing \"data.cgf\" file!", message = $"The file \"data.cgf\" is missing! It may have been renamed, moved, or deleted. Do you want to download the installer again?";
                        if (DownloadedLocalizationList)
                        {
                            title = Localizations.GetLString(INIReader.GetString("config.ini", "CONFIG", "language", "English"), "0-92");
                            message = Localizations.GetLString(INIReader.GetString("config.ini", "CONFIG", "language", "English"), "0-93");
                        }
                        if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
                        {
                            if (!File.Exists("UpdateDownloader.exe"))
                            {
                                message = "UpdateDownloader.exe has been deleted, renamed, or moved. After closing this message, it will be downloaded again.";
                                string caption = "Error";
                                if (DownloadedLocalizationList)
                                {
                                    caption = Localizations.GetLString(INIReader.GetString("config.ini", "CONFIG", "language", "English"), "0-94");
                                    message = Localizations.GetLString(INIReader.GetString("config.ini", "CONFIG", "language", "English"), "0-95");
                                }
                                MessageBox.Show(message, caption, MessageBoxButtons.OK);
                                await DownloadFileAsync("https://base-escape.ru/downloads/UpdateDownloader.exe", "UpdateDownloader.exe");
                            }
                            Process.Start(new ProcessStartInfo("UpdateDownloader.exe", "https://base-escape.ru/downloads/Setup_SLIL.exe Setup_SLIL"));
                            Application.Exit();
                        }
                        else
                            Application.Exit();
                    }
                }
                if (Stage == 3)
                {
                    status_label.Text = "Texture caching...";
                    textureCache = new TextureCache();
                    await ProcessTexturesWithProgressAsync();
                    Stage++;
                }
                if (Stage == 4)
                {
                    status_label.Text = "Loading main menu...";
                    mainMenu = await CreateMainMenuAsync(this);
                    mainMenu.FormClosing += MainMenu_FormCLosing;
                    progress_refresh.Stop();
                    mainMenu.Show();
                    Hide();
                }
            }
            catch { }
        }

        public static async Task<MainMenu> CreateMainMenuAsync(Loading loading)
        {
            return await Task.Run(() =>
            {
                MainMenu mainMenu = new MainMenu(loading.CGFReader, loading.textureCache)
                {
                    UpdateVerified = loading.UpdateVerified,
                    downloadedLocalizationList = loading.DownloadedLocalizationList,
                    localizations = Localizations,
                    supportedLanguages = loading.SupportedLanguages
                };
                return mainMenu;
            });
        }

        private void Progress_refresh_Tick(object sender, EventArgs e)
        {
            if (CompletedProgress < UpdateProgress)
                CompletedProgress++;
            progress.Width = (int)(CompletedProgress / 100 * background_progress.Width);
        }

        private async void Loading_Load(object sender, EventArgs e) => await LoadingMainMenu();

        private void MainMenu_FormCLosing(object sender, FormClosingEventArgs e) => Application.Exit();
    }
}