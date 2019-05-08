using System;

namespace RewriteMe.Domain.Exceptions
{
    public class OfflineRequestException : Exception
    {
        public OfflineRequestException()
        {
        }

        public OfflineRequestException(string message)
            : base(message)
        {
        }

        public OfflineRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
