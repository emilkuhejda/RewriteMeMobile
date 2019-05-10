using System;

namespace RewriteMe.DataAccess.Entities
{
    public class BillingPurchaseEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string PurchaseId { get; set; }

        public string ProductId { get; set; }

        public string AutoRenewing { get; set; }

        public string PurchaseState { get; set; }

        public string ConsumptionState { get; set; }

        public DateTime TransactionDateUtc { get; set; }
    }
}
