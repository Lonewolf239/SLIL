using SLIL.SLIL_Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SLIL
{
    public partial class Loading : Form
    {
        private readonly string current_version = Program.current_version;
        private int sec = 0;
        private bool UpdateVerified = false, CurrentVersion = false;
        private bool DownloadedLocalizationList = false;
        public static Localization Localizations = new Localization();
        private readonly Dictionary<string, string> SupportedLanguages = new Dictionary<string, string>();

        public Loading() => InitializeComponent();

        private void SetLocalizations(string[] codes, string[] languages)
        {
            SupportedLanguages.Clear();
            for (int i = 0; i < languages.Length; i++)
                SupportedLanguages.Add(codes[i], languages[i]);
            for (int i = 0; i < languages.Length; i++)
            {
                try
                {
                    Localizations.DownloadLocalization(languages[i]);
                }
                catch
                {
                    DownloadedLocalizationList = false;
                    return;
                }
            }
            DownloadedLocalizationList = true;
        }

        private void DownloadLocalizationList()
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                try
                {
                    string result = webClient.DownloadString(new Uri("https://base-escape.ru/SLILLocalization/LocalizationList.txt"));
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
                        SetLocalizations(codes, languages);
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

        private void DownloadFile(string url, string outputPath)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(new Uri(url), outputPath);
                }
                catch { }
            }
        }

        private void Check_Update()
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    string line = webClient.DownloadString(new Uri("https://base-escape.ru/version_SLIL.txt")).Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)[0];
                    if (!line.Contains(current_version))
                    {
                        if (!File.Exists("UpdateDownloader.exe"))
                            DownloadFile("https://base-escape.ru/downloads/UpdateDownloader.exe", "UpdateDownloader.exe");
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

        private void Start_timer_Tick(object sender, EventArgs e)
        {
            status_label.Text = "Loading game resources...";
            if (sec == 1)
            {
                start_timer.Stop();
                DownloadLocalizationList();
                MainMenu form = new MainMenu()
                {
                    UpdateVerified = UpdateVerified,
                    downloadedLocalizationList = DownloadedLocalizationList,
                    localizations = Localizations,
                    supportedLanguages = SupportedLanguages
                };
                form.FormClosing += new FormClosingEventHandler(MainMenu_FormCLosing);
                form.Show();
                Hide();
            }
            sec++;
        }

        private void Loading_Load(object sender, EventArgs e)
        {
            Check_Update();
            if (CurrentVersion)
                start_timer.Start();
            else
            {
                Process.Start(new ProcessStartInfo("UpdateDownloader.exe", "https://base-escape.ru/downloads/Setup_SLIL.exe Setup_SLIL"));
                Application.Exit();
            }
        }

        private void MainMenu_FormCLosing(object sender, FormClosingEventArgs e) => Application.Exit();
    }
}