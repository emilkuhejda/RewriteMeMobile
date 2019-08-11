using System;
using System.Runtime.Serialization;

namespace RewriteMe.Domain.Exceptions
{
    public class UnauthorizedCallException : Exception
    {
        public UnauthorizedCallException()
        {
        }

        public UnauthorizedCallException(string message)
            : base(message)
        {
        }

        public UnauthorizedCallException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnauthorizedCallException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
