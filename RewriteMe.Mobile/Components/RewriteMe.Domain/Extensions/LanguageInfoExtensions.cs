using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Localization;

namespace RewriteMe.Domain.Extensions
{
    public static class LanguageInfoExtensions
    {
        public static Language ToLanguage(this LanguageInfo languageInfo)
        {
            if (languageInfo.Culture == "en")
                return Language.English;

            if (languageInfo.Culture == "sk")
                return Language.Slovak;

            return Language.Undefined;
        }
    }
}
