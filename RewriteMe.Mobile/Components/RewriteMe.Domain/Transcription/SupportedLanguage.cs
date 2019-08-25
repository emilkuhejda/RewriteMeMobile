namespace RewriteMe.Domain.Transcription
{
    public class SupportedLanguage
    {
        public SupportedLanguage(string title, string culture, bool isAzureSupported)
            : this(title, culture, isAzureSupported, false)
        {
        }

        public SupportedLanguage(string title, string culture, bool isAzureSupported, bool onlyInAzure)
        {
            Title = title;
            Culture = culture;
            IsAzureSupported = isAzureSupported;
            OnlyInAzure = onlyInAzure;
        }

        public string Title { get; }

        public string Culture { get; }

        public bool IsAzureSupported { get; }

        public bool OnlyInAzure { get; }
    }
}
