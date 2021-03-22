using System;

namespace RewriteMe.Domain.Exceptions
{
    public class PurchaseNotFoundException : Exception
    {
        public PurchaseNotFoundException()
        {
        }

        public PurchaseNotFoundException(string purchaseId, string productId)
        {
            PurchaseId = purchaseId;
            ProductId = productId;
        }

        public PurchaseNotFoundException(string message)
            : base(message)
        {
            PurchaseId = string.Empty;
            ProductId = string.Empty;
        }

        public PurchaseNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            PurchaseId = string.Empty;
            ProductId = string.Empty;
        }

        public string PurchaseId { get; }

        public string ProductId { get; }
    }
}
