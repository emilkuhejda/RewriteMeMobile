using RewriteMe.Domain.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Transcription
{
    public static class SubscriptionProducts
    {
        public static SubscriptionProduct ProductOneHour { get; } = new SubscriptionProduct(
            "product.subscription.basic",
            Loc.Text(TranslationKeys.OneHour),
            "resource://RewriteMe.Mobile.Resources.Images.Subscription-1.svg");

        public static SubscriptionProduct ProductTenHours { get; } = new SubscriptionProduct(
            "product.subscription.advanced",
            Loc.Text(TranslationKeys.TenHours),
            "resource://RewriteMe.Mobile.Resources.Images.Subscription-10.svg");

        public static SubscriptionProduct[] All { get; } = { ProductOneHour, ProductTenHours };
    }
}
