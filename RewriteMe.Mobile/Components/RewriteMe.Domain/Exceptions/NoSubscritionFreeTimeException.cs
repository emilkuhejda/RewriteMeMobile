using System;

namespace RewriteMe.Domain.Exceptions
{
    public class NoSubscritionFreeTimeException : Exception
    {
        public NoSubscritionFreeTimeException()
        {
        }

        public NoSubscritionFreeTimeException(string message)
            : base(message)
        {
        }

        public NoSubscritionFreeTimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
