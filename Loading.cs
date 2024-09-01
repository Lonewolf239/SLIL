using SLIL.SLIL_Localization;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace SLIL
{
    public partial class Loading : Form
    {
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

        private void Start_timer_Tick(object sender, EventArgs e)
        {
            start_timer.Stop();
            DownloadLocalizationList();
            MainMenu form = new MainMenu()
            {
                downloadedLocalizationList = DownloadedLocalizationList,
                localizations = Localizations,
                supportedLanguages = SupportedLanguages
            };
            form.FormClosing += new FormClosingEventHandler(MainMenu_FormCLosing);
            form.Show();
            Hide();
        }

        private void MainMenu_FormCLosing(object sender, FormClosingEventArgs e) => Application.Exit();
    }
}