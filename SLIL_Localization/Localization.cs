using System;
using System.Text;
using System.Net;
using System.Collections.Generic;

namespace SLIL.SLIL_Localization
{
    public class Localization
    {
        private readonly List<SLILLocalization> SLILLocalizations;

        public Localization() => SLILLocalizations = new List<SLILLocalization>();

        public void DownloadLocalization(string language)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                try
                {
                    string result = webClient.DownloadString($"https://base-escape.ru/SLILLocalization/{language}.txt");
                    ProcessDownloadedData(language, result);
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ConnectFailure)
                        throw new Exception("Failed to establish a connection with the localizations server. Please check your internet connection.");
                    else
                        throw new Exception($"An error occurred while downloading the localization: {e.Message}");
                }
                catch (Exception e)
                {
                    throw new Exception($"An unexpected error occurred: {e.Message}");
                }
            }
        }

        private void ProcessDownloadedData(string language, string data)
        {
            string[] lines = data.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            if (lines.Length > 0)
            {
                List<LocalizationString> strings = new List<LocalizationString>();
                foreach (var line in lines)
                {
                    string[] parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        strings.Add(new LocalizationString
                        {
                            LIndex = parts[0],
                            LString = parts[1]
                        });
                    }
                }
                lock (SLILLocalizations)
                    SLILLocalizations.Add(new SLILLocalization(language, strings));
            }
        }

        public string GetLString(string language, string index)
        {
            bool all_ok = false;
            int i = 0;
            while (!all_ok)
            {
                for (i = 0; i < SLILLocalizations.Count; i++)
                {
                    if (SLILLocalizations[i].Language == language)
                    {
                        all_ok = true;
                        break;
                    }
                }
                if (!all_ok) try { DownloadLocalization(language); } catch { return "Error"; }
            }
            return SLILLocalizations[i].GetString(index);
        }

        public void RemoveDuplicates()
        {
            List<SLILLocalization> temp = new List<SLILLocalization>(SLILLocalizations);
            SLILLocalizations.Clear();
            foreach (SLILLocalization localization in temp)
            {
                bool all_ok = true;
                for (int i = 0; i < SLILLocalizations.Count; i++)
                {
                    if (SLILLocalizations[i].Language == localization.Language)
                    {
                        all_ok = false;
                        break;
                    }
                }
                if (all_ok) SLILLocalizations.Add(localization);
            }
        }
    }
}