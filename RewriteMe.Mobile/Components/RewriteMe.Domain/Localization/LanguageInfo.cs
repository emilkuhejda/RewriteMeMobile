using System.Globalization;

namespace RewriteMe.Domain.Localization
{
    public class LanguageInfo
    {
        public LanguageInfo(string key, string culture)
        {
            Key = key;
            Culture = culture;
        }

        public string Key { get; }

        public string Culture { get; }

        public CultureInfo GetCultureInfo()
        {
            return new CultureInfo(Culture);
        }
    }
}
