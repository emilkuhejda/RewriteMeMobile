namespace RewriteMe.Domain.Transcription
{
    public class SupportedLanguage
    {
        public SupportedLanguage(string title, string culture)
        {
            Title = title;
            Culture = culture;
        }

        public string Title { get; }

        public string Culture { get; }
    }
}
