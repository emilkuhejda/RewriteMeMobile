using System.Collections.Generic;

namespace RewriteMe.Mobile.Transcription
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/cognitive-services/speech-service/language-support
    /// </summary>
    public static class AzureSupportedLanguages
    {
        public static bool IsSupported(string language)
        {
            return Languages.Contains(language);
        }

        private static IList<string> Languages { get; } = new List<string>
        {
            "ar-EG",
            "ca-ES",
            "da-DK",
            "de-DE",
            "en-AU",
            "en-CA",
            "en-GB",
            "en-IN",
            "en-NZ",
            "en-US",
            "es-ES",
            "es-MX",
            "fi-FI",
            "fr-CA",
            "fr-FR",
            "hi-IN",
            "it-IT",
            "ja-JP",
            "ko-KR",
            "nb-NO",
            "nl-NL",
            "pl-PL",
            "pt-BR",
            "pt-PT",
            "ru-RU",
            "sv-SE",
            "zh-CN",
            "zh-HK",
            "zh-TW",
            "th-TH"
        };
    }
}
