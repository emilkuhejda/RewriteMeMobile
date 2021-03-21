using Plugin.InAppBilling;
using RewriteMe.DataAccess.Entities;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class BillingPurchaseDataAdapter
    {
        public static InAppBillingPurchase ToBillingPurchase(this BillingPurchaseEntity entity)
        {
            return new InAppBillingPurchase
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                AutoRenewing = entity.AutoRenewing,
                PurchaseToken = entity.PurchaseToken,
                State = entity.State,
                ConsumptionState = entity.ConsumptionState,
                IsAcknowledged = entity.IsAcknowledged,
                TransactionDateUtc = entity.TransactionDateUtc
            };
        }

        public static BillingPurchaseEntity ToBillingPurchaseEntity(this InAppBillingPurchase billingPurchase)
        {
            return new BillingPurchaseEntity
            {
                Id = billingPurchase.Id,
                ProductId = billingPurchase.ProductId,
                AutoRenewing = billingPurchase.AutoRenewing,
                PurchaseToken = billingPurchase.PurchaseToken,
                State = billingPurchase.State,
                ConsumptionState = billingPurchase.ConsumptionState,
                IsAcknowledged = billingPurchase.IsAcknowledged,
                TransactionDateUtc = billingPurchase.TransactionDateUtc
            };
        }
    }
}
