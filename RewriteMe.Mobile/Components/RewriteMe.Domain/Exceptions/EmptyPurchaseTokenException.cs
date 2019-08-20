using System;

namespace RewriteMe.Domain.Exceptions
{
    public class EmptyPurchaseTokenException : PurchaseBaseException
    {
        public EmptyPurchaseTokenException()
        {
        }

        public EmptyPurchaseTokenException(string message)
            : base(message)
        {
        }

        public EmptyPurchaseTokenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public EmptyPurchaseTokenException(string purchaseId, string productId)
            : base(purchaseId, productId)
        {
        }
    }
}
