using System;

namespace RewriteMe.Domain.Exceptions
{
    public class NoPurchasesInStoreException : Exception
    {
        public NoPurchasesInStoreException()
        {
        }

        public NoPurchasesInStoreException(string message)
            : base(message)
        {
        }

        public NoPurchasesInStoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
