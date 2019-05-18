using System;

namespace RewriteMe.Domain.Exceptions
{
    public abstract class PurchaseBaseException : Exception
    {
        protected PurchaseBaseException()
        {
        }

        protected PurchaseBaseException(string message)
            : base(message)
        {
        }

        protected PurchaseBaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PurchaseBaseException(string purchaseId, string productId)
        {
            PurchaseId = purchaseId;
            ProductId = productId;
        }

        protected PurchaseBaseException(string purchaseId, string productId, string message, Exception innerException)
            : base(message, innerException)
        {
            PurchaseId = purchaseId;
            ProductId = productId;
        }

        public string PurchaseId { get; }

        public string ProductId { get; }
    }
}
