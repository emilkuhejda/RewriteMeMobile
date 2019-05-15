using System;

namespace RewriteMe.Domain.Exceptions
{
    public class InAppBillingNotSupportedException : Exception
    {
        public InAppBillingNotSupportedException()
        {
        }

        public InAppBillingNotSupportedException(string message)
            : base(message)
        {
        }

        public InAppBillingNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
