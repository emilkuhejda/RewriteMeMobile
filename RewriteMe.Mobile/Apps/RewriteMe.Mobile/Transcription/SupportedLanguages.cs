using RewriteMe.Domain.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Transcription
{
    public static class SupportedLanguages
    {
        public static SupportedLanguage EnglishAu { get; } = new SupportedLanguage(TranslationKeys.LanguageEnAu, "en-AU", true);

        public static SupportedLanguage EnglishGb { get; } = new SupportedLanguage(TranslationKeys.LanguageEnGb, "en-GB", true);

        public static SupportedLanguage EnglishUs { get; } = new SupportedLanguage(TranslationKeys.LanguageEnUs, "en-US", true);

        public static SupportedLanguage German { get; } = new SupportedLanguage(TranslationKeys.LanguageDeDe, "de-DE", true);

        public static SupportedLanguage French { get; } = new SupportedLanguage(TranslationKeys.LanguageFrFr, "fr-FR", true);

        public static SupportedLanguage Spanish { get; } = new SupportedLanguage(TranslationKeys.LanguageEsEs, "es-ES", true);

        public static SupportedLanguage Italian { get; } = new SupportedLanguage(TranslationKeys.LanguageItIt, "it-IT", true);

        public static SupportedLanguage Portuguese { get; } = new SupportedLanguage(TranslationKeys.LanguagePtPt, "pt-PT", true);

        public static SupportedLanguage Slovak { get; } = new SupportedLanguage(TranslationKeys.LanguageSkSk, "sk-SK", false);

        public static SupportedLanguage Czech { get; } = new SupportedLanguage(TranslationKeys.LanguageCsCz, "cs-CZ", false);

        public static SupportedLanguage Polish { get; } = new SupportedLanguage(TranslationKeys.LanguagePlPl, "pl-PL", true);

        public static SupportedLanguage Hungarian { get; } = new SupportedLanguage(TranslationKeys.LanguageHuHu, "hu-HU", false);

        public static SupportedLanguage Russian { get; } = new SupportedLanguage(TranslationKeys.LanguageRuRu, "ru-RU", true);

        public static SupportedLanguage Japanese { get; } = new SupportedLanguage(TranslationKeys.LanguageJaJp, "ja-JP", true);

        public static SupportedLanguage ChineseCn { get; } = new SupportedLanguage(TranslationKeys.LanguageZhCn, "zh-CN", true, true);

        public static SupportedLanguage Chinese { get; } = new SupportedLanguage(TranslationKeys.LanguageZh, "zh", false);

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
