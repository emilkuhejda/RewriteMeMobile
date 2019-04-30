using RewriteMe.DataAccess.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Transcription
{
    public static class SupportedLanguages
    {
        public static SupportedLanguage EnglishGB { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.English), "en-GB");

        public static SupportedLanguage EnglishUS { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.English), "es-US");

        public static SupportedLanguage Slovak { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.Slovak), "sk-SK");

        public static SupportedLanguage[] All => new[] { EnglishGB, EnglishUS, Slovak };
    }
}
