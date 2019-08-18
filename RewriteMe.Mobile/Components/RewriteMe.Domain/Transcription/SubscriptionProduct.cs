namespace RewriteMe.Domain.Transcription
{
    public class SubscriptionProduct
    {
        public SubscriptionProduct(string id, string text, string iconKey)
        {
            Id = id;
            Text = text;
            IconKey = iconKey;
        }

        public string Id { get; }

        public string Text { get; }

        public string IconKey { get; }
    }
}
