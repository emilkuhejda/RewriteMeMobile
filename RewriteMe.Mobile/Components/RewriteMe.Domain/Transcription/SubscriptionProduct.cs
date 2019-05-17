using System;

namespace RewriteMe.Domain.Transcription
{
    public class SubscriptionProduct
    {
        public SubscriptionProduct(string id, string text, TimeSpan time)
        {
            Id = id;
            Text = text;
            Time = time;
        }

        public string Id { get; }

        public string Text { get; }

        public TimeSpan Time { get; }
    }
}
