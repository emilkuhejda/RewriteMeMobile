using RewriteMe.Domain.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.Transcription
{
    public static class SubscriptionProducts
    {
        public static SubscriptionProduct ProductBasic { get; } = new SubscriptionProduct(
            "product.subscription.v1.basic",
            Loc.Text(TranslationKeys.OneHour));

        public static SubscriptionProduct ProductStandard { get; } = new SubscriptionProduct(
            "product.subscription.v1.standard",
            Loc.Text(TranslationKeys.FiveHours),
            Loc.Text(TranslationKeys.DescriptionFiveHours));

        public static SubscriptionProduct ProductAdvanced { get; } = new SubscriptionProduct(
            "product.subscription.v1.advanced",
            Loc.Text(TranslationKeys.TenHours),
            Loc.Text(TranslationKeys.DescriptionTenHours));

        public static SubscriptionProduct[] All { get; } = { ProductBasic, ProductStandard, ProductAdvanced };
    }
}
