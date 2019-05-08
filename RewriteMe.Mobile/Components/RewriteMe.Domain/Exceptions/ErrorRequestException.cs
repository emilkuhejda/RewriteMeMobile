using System;

namespace RewriteMe.Domain.Exceptions
{
    public class ErrorRequestException : Exception
    {
        public ErrorRequestException()
        {
        }

        public ErrorRequestException(string message)
            : base(message)
        {
        }

        public ErrorRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ErrorRequestException(int? statusCode)
        {
            StatusCode = statusCode;
        }

        public int? StatusCode { get; }
    }
}
