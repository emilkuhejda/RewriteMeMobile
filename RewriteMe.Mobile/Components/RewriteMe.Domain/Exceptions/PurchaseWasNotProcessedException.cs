using System;

namespace RewriteMe.Domain.Exceptions
{
    public class PurchaseWasNotProcessedException : Exception
    {
        public PurchaseWasNotProcessedException()
        {
        }

        public PurchaseWasNotProcessedException(string message)
            : base(message)
        {
        }

        public PurchaseWasNotProcessedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
