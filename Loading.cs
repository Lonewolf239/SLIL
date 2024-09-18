using SLIL.SLIL_Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLIL
{
    public partial class Loading : Form
    {
        private readonly string current_version = Program.current_version;
        private int Stage = 0;
        private bool UpdateVerified = false, CurrentVersion = false;
        private bool DownloadedLocalizationList = false;
        public static Localization Localizations = new Localization();
        private readonly Dictionary<string, string> SupportedLanguages = new Dictionary<string, string>();

        public Loading() => InitializeComponent();

        private async Task SetLocalizationsAsync(string[] codes, string[] languages)
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
                        await SetLocalizationsAsync(codes, languages);
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
                            {
                                await stream.CopyToAsync(fileStream);
                            }
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

        private async Task LoadingMainMenu()
        {
            try
            {
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
                }
                if (Stage == 1)
                {
                    status_label.Text = "Downloading localization...";
                    await DownloadLocalizationList();
                    Stage++;
                }
                if (Stage == 2)
                {
                    status_label.Text = "Loading game resources...";
                    MainMenu mainMenu = await CreateMainMenuAsync(this);
                    mainMenu.FormClosing += MainMenu_FormCLosing;
                    mainMenu.Show();
                    Hide();
                    Stage++;
                }
            }
            catch { }
        }

        public static async Task<MainMenu> CreateMainMenuAsync(Loading loading)
        {
            return await Task.Run(() =>
            {
                MainMenu mainMenu = new MainMenu()
                {
                    UpdateVerified = loading.UpdateVerified,
                    downloadedLocalizationList = loading.DownloadedLocalizationList,
                    localizations = Localizations,
                    supportedLanguages = loading.SupportedLanguages
                };
                return mainMenu;
            });
        }

        private async void Loading_Load(object sender, EventArgs e) => await LoadingMainMenu();

        private void MainMenu_FormCLosing(object sender, FormClosingEventArgs e) => Application.Exit();
    }
}