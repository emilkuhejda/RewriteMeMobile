using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class BillingPurchaseDataAdapter
    {
        public static BillingPurchase ToBillingPurchase(this BillingPurchaseEntity entity)
        {
            return new BillingPurchase
            {
                Id = entity.Id,
                UserId = entity.UserId,
                PurchaseId = entity.PurchaseId,
                ProductId = entity.ProductId,
                AutoRenewing = entity.AutoRenewing,
                PurchaseState = entity.PurchaseState,
                ConsumptionState = entity.ConsumptionState,
                TransactionDateUtc = entity.TransactionDateUtc
            };
        }

        public static BillingPurchaseEntity ToBillingPurchaseEntity(this BillingPurchase billingPurchase)
        {
            return new BillingPurchaseEntity
            {
                Id = billingPurchase.Id.GetValueOrDefault(),
                UserId = billingPurchase.UserId.GetValueOrDefault(),
                PurchaseId = billingPurchase.PurchaseId,
                ProductId = billingPurchase.ProductId,
                AutoRenewing = billingPurchase.AutoRenewing,
                PurchaseState = billingPurchase.PurchaseState,
                ConsumptionState = billingPurchase.ConsumptionState,
                TransactionDateUtc = billingPurchase.TransactionDateUtc.GetValueOrDefault()
            };
        }
    }
}
