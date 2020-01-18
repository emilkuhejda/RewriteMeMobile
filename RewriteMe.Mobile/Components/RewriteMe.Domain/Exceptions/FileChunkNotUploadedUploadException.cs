using System;

namespace RewriteMe.Domain.Exceptions
{
    public class FileChunkNotUploadedUploadException : Exception
    {
        public FileChunkNotUploadedUploadException()
        {
        }

        public FileChunkNotUploadedUploadException(string message)
            : base(message)
        {
        }

        public FileChunkNotUploadedUploadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
