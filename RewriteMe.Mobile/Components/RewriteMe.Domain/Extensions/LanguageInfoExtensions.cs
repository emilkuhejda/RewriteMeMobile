using System;
using System.Globalization;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Localization;

namespace RewriteMe.Domain.Extensions
{
    public static class LanguageInfoExtensions
    {
        public static Language ToLanguage(this CultureInfo cultureInfo)
        {
            return ConvertToLanguage(cultureInfo.TwoLetterISOLanguageName);
        }

        public static Language ToLanguage(this LanguageInfo languageInfo)
        {
            return ConvertToLanguage(languageInfo.Culture);
        }

        private static Language ConvertToLanguage(string language)
        {
            if (language.Equals("en", StringComparison.OrdinalIgnoreCase))
                return Language.English;

            if (language.Equals("sk", StringComparison.OrdinalIgnoreCase))
                return Language.Slovak;

            return Language.Undefined;
        }
    }
}
