using System.Globalization;

namespace RewriteMe.Domain.Localization
{
    public class LanguageInfo
    {
        public LanguageInfo(string title, string culture)
        {
            Title = title;
            Culture = culture;
            FullName = string.Format(CultureInfo.InvariantCulture, "{0} ({1})", Title, Culture);
        }

        public string Title { get; }

        public string Culture { get; }

        public string FullName { get; }

        public CultureInfo GetCultureInfo()
        {
            return new CultureInfo(Culture);
        }
    }
}
