using System;
using Plugin.InAppBilling;

namespace RewriteMe.DataAccess.Entities
{
    public class BillingPurchaseEntity
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public bool AutoRenewing { get; set; }

        public string PurchaseToken { get; set; }

        public PurchaseState State { get; set; }

        public ConsumptionState ConsumptionState { get; set; }

        public bool IsAcknowledged { get; set; }

        public DateTime TransactionDateUtc { get; set; }
    }
}
