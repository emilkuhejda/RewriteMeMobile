namespace RewriteMe.Domain.Transcription
{
    public class SubscriptionProduct
    {
        public SubscriptionProduct(string productId, string text, string iconKey)
        {
            ProductId = productId;
            Text = text;
            IconKey = iconKey;
        }

        public string ProductId { get; }

        public string Text { get; }

        public string IconKey { get; }
    }
}
