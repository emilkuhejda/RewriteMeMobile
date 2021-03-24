using System;

namespace RewriteMe.Domain.Exceptions
{
    public class PurchaseWasNotProcessedException : PurchaseBaseException
    {
        public PurchaseWasNotProcessedException()
        {
        }

        public PurchaseWasNotProcessedException(string message)
            : base(message)
        {
        }

        public PurchaseWasNotProcessedException(string purchaseId, string productId)
            : base(purchaseId, productId)
        {
        }

        public PurchaseWasNotProcessedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
