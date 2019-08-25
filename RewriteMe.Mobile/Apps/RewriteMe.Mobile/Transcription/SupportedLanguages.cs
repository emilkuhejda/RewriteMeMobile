using RewriteMe.Domain.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Transcription
{
    public static class SupportedLanguages
    {
        public static SupportedLanguage EnglishAu { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageEnAu), "en-AU", true);

        public static SupportedLanguage EnglishGb { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageEnGb), "en-GB", true);

        public static SupportedLanguage EnglishUs { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageEnUs), "en-US", true);

        public static SupportedLanguage German { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageDeDe), "de-DE", true);

        public static SupportedLanguage French { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageFrFr), "fr-FR", true);

        public static SupportedLanguage Spanish { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageEsEs), "es-ES", true);

        public static SupportedLanguage Italian { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageItIt), "it-IT", true);

        public static SupportedLanguage Portuguese { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguagePtPt), "pt-PT", true);

        public static SupportedLanguage Slovak { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageSkSk), "sk-SK", false);

        public static SupportedLanguage Czech { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageCsCz), "cs-CZ", false);

        public static SupportedLanguage Polish { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguagePlPl), "pl-PL", true);

        public static SupportedLanguage Hungarian { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageHuHu), "hu-HU", false);

        public static SupportedLanguage Russian { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageRuRu), "ru-RU", true);

        public static SupportedLanguage Japanese { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageJaJp), "ja-JP", true);

        public static SupportedLanguage ChineseCn { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageZhCn), "zh-CN", true, true);

        public static SupportedLanguage Chinese { get; } = new SupportedLanguage(Loc.Text(TranslationKeys.LanguageZh), "zh", false);

        public static SupportedLanguage[] All => new[]
        {
            EnglishAu,
            EnglishGb,
            EnglishUs,
            German,
            French,
            Spanish,
            Italian,
            Portuguese,
            Slovak,
            Czech,
            Polish,
            Hungarian,
            Russian,
            Japanese,
            ChineseCn,
            Chinese
        };
    }
}
