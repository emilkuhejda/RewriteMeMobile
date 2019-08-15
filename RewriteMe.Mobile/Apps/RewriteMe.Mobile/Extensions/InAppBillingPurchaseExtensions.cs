using System;
using Plugin.InAppBilling.Abstractions;
using RewriteMe.Domain.WebApi.Models;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Extensions
{
    public static class InAppBillingPurchaseExtensions
    {
        public static BillingPurchase ToBillingPurchase(this InAppBillingPurchase inAppBillingPurchase, Guid userId, string orderId)
        {
            return new BillingPurchase
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PurchaseId = orderId,
                ProductId = inAppBillingPurchase.ProductId,
                AutoRenewing = inAppBillingPurchase.AutoRenewing,
                PurchaseState = inAppBillingPurchase.State.ToString(),
                ConsumptionState = inAppBillingPurchase.ConsumptionState.ToString(),
                Platform = Device.RuntimePlatform,
                TransactionDateUtc = inAppBillingPurchase.TransactionDateUtc
            };
        }
    }
}
