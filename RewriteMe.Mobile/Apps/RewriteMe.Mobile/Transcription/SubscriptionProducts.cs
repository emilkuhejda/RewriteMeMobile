using System;
using RewriteMe.Domain.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Transcription
{
    public static class SubscriptionProducts
    {
        public static SubscriptionProduct ProductOneHour { get; } = new SubscriptionProduct("product.subscription.1hour", Loc.Text(TranslationKeys.OneHour), TimeSpan.FromHours(1));

        public static SubscriptionProduct[] All { get; } = { ProductOneHour };
    }
}
