using System;

namespace RewriteMe.Domain.Exceptions
{
    public class PurchasePayloadNotValidException : Exception
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
    }
}
