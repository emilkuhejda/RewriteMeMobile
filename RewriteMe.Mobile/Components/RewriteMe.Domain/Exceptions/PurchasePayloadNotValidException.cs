using System;

namespace RewriteMe.Domain.Exceptions
{
    public class PurchasePayloadNotValidException : PurchaseBaseException
    {
        public PurchasePayloadNotValidException()
        {
        }

        public PurchasePayloadNotValidException(string message)
            : base(message)
        {
        }

        public PurchasePayloadNotValidException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public PurchasePayloadNotValidException(string purchaseId, string productId)
            : base(purchaseId, productId)
        {
        }
    }
}
