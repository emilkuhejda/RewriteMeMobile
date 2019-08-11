using System;
using System.Runtime.Serialization;

namespace RewriteMe.Domain.Exceptions
{
    public class UserRegistrationFailedException : Exception
    {
        public UserRegistrationFailedException()
        {
        }

        public UserRegistrationFailedException(string message)
            : base(message)
        {
        }

        public UserRegistrationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UserRegistrationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
