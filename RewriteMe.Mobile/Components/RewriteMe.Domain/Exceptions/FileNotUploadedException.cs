using System;

namespace RewriteMe.Domain.Exceptions
{
    public class FileNotUploadedException : Exception
    {
        public FileNotUploadedException()
        {
        }

        public FileNotUploadedException(string message)
            : base(message)
        {
        }

        public FileNotUploadedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
