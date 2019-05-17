using System;

namespace RewriteMe.Domain.Exceptions
{
    public class AppStoreNotConnectedException : Exception
    {
        public AppStoreNotConnectedException()
        {
        }

        public AppStoreNotConnectedException(string message)
            : base(message)
        {
        }

        public AppStoreNotConnectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
