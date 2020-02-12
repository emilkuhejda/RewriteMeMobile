using System;
using RewriteMe.Domain.WebApi;

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

        public ErrorRequestException(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public ErrorCode ErrorCode { get; }
    }
}
