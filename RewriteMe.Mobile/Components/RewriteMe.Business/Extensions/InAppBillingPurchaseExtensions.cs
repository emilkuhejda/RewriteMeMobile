using System;
using Plugin.InAppBilling;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Extensions
{
    public static class InAppBillingPurchaseExtensions
    {
        public static CreateUserSubscriptionInputModel ToUserSubscriptionModel(this InAppBillingPurchase inAppBillingPurchase, Guid userId, string orderId, string runtimePlatform)
        {
            return new CreateUserSubscriptionInputModel
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PurchaseId = orderId,
                ProductId = inAppBillingPurchase.ProductId,
                AutoRenewing = inAppBillingPurchase.AutoRenewing,
                PurchaseToken = inAppBillingPurchase.PurchaseToken,
                PurchaseState = inAppBillingPurchase.State.ToString(),
                ConsumptionState = inAppBillingPurchase.ConsumptionState.ToString(),
                Platform = runtimePlatform,
                TransactionDateUtc = inAppBillingPurchase.TransactionDateUtc
            };
        }
    }
}
