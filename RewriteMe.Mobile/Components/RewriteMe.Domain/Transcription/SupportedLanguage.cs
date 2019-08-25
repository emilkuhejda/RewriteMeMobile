namespace RewriteMe.Domain.Transcription
{
    public class SupportedLanguage
    {
        public SupportedLanguage(string title, string culture, bool isAzureSupported)
        {
            Title = title;
            Culture = culture;
            IsAzureSupported = isAzureSupported;
        }

        public string Title { get; }

        public string Culture { get; }

        public bool IsAzureSupported { get; }
    }
}
