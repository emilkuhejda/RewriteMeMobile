namespace RewriteMe.Domain.Transcription
{
    public class SubscriptionProduct
    {
        public SubscriptionProduct(string productId, string text)
            : this(productId, text, string.Empty)
        {
        }

        public SubscriptionProduct(string productId, string text, string description)
        {
            ProductId = productId;
            Text = text;
            Description = description;
        }

        public string ProductId { get; }

        public string Text { get; }

        public string Description { get; }
    }
}
