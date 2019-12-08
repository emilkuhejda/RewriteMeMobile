using RewriteMe.Domain.Localization;

namespace RewriteMe.Resources.Localization
{
    public static class Languages
    {
        public static LanguageInfo English { get; } = new LanguageInfo(TranslationKeys.English, "en");

        public static LanguageInfo Slovak { get; } = new LanguageInfo(TranslationKeys.Slovak, "sk");

        public static LanguageInfo[] All => new[] { English, Slovak };
    }
}
