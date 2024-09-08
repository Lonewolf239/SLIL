using System.Collections.Generic;

namespace SLIL.SLIL_Localization
{
    public class SLILLocalization
    {
        private readonly List<LocalizationString> LocalizationStrings;
        public string Language;

        public SLILLocalization(string language, List<LocalizationString> localizationStrings)
        {
            Language = language;
            LocalizationStrings = localizationStrings;
        }

        public string GetString(string index)
        {
            for (int i = 0; i < LocalizationStrings.Count; i++)
            {
                if (LocalizationStrings[i].LIndex == index)
                    return LocalizationStrings[i].LString.Replace("&", "\n");
            }
            return "Error";
        }
    }
}