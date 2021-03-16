using System;
using Plugin.InAppBilling.Abstractions;
using RewriteMe.Domain.WebApi;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Extensions
{
    public static class InAppBillingPurchaseExtensions
    {
        public static CreateUserSubscriptionInputModel ToUserSubscriptionModel(this InAppBillingPurchase inAppBillingPurchase, Guid userId, string orderId)
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
                Platform = Device.RuntimePlatform,
                TransactionDateUtc = inAppBillingPurchase.TransactionDateUtc
            };
        }
    }
}
