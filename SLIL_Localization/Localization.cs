using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SLIL.SLIL_Localization
{
    public class Localization
    {
        private readonly List<SLILLocalization> SLILLocalizations;

        public Localization() => SLILLocalizations = new List<SLILLocalization>();

        public async Task DownloadLocalization(string language)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/text"));
                    HttpResponseMessage response = await httpClient.GetAsync($"https://base-escape.ru/SLILLocalization/{language}.txt");
                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        ProcessDownloadedData(language, result);
                    }
                    else
                    {
                        throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException e)
                {
                    if (e.InnerException is System.Net.Sockets.SocketException)
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
                    if (line.StartsWith(";") || line.Length < 3) continue;
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

        public async Task<string> GetString(string language, string index)
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
                if (!all_ok) try { await DownloadLocalization(language); } catch { return "Error"; }
            }
            return SLILLocalizations[i].GetString(index);
        }

        public string GetLString(string language, string index) => GetString(language, index).Result;

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