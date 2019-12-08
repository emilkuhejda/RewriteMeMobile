namespace RewriteMe.Domain.Transcription
{
    public class SupportedLanguage
    {
        public SupportedLanguage(string key, string culture, bool isAzureSupported)
            : this(key, culture, isAzureSupported, false)
        {
        }

        public SupportedLanguage(string key, string culture, bool isAzureSupported, bool onlyInAzure)
        {
            Key = key;
            Culture = culture;
            IsAzureSupported = isAzureSupported;
            OnlyInAzure = onlyInAzure;
        }

        public string Key { get; }

        public string Culture { get; }

        public bool IsAzureSupported { get; }

        public bool OnlyInAzure { get; }
    }
}
