using RewriteMe.Domain.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Transcription
{
    public static class SubscriptionProducts
    {
        public static SubscriptionProduct ProductBasic { get; } = new SubscriptionProduct(
            "product.subscription.basic",
            Loc.Text(TranslationKeys.OneHour));

        public static SubscriptionProduct ProductAdvanced { get; } = new SubscriptionProduct(
            "product.subscription.advanced",
            Loc.Text(TranslationKeys.TenHours),
            Loc.Text(TranslationKeys.DescriptionTenHours));

        public static SubscriptionProduct[] All { get; } = { ProductBasic, ProductAdvanced };
    }
}
