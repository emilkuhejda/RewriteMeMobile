using RewriteMe.Domain.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Transcription
{
    public static class SupportedLanguages
    {
        public static SupportedLanguage EnglishGb { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageEnGb), "en-GB");

        public static SupportedLanguage EnglishUs { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageEnUs), "en-US");

        public static SupportedLanguage Slovak { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageSkSk), "sk-SK");

        public static SupportedLanguage[] All => new[] { EnglishGb, EnglishUs, Slovak };
    }
}
