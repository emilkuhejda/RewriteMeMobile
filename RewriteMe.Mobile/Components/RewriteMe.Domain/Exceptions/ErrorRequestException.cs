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

        public ErrorRequestException(int? statusCode, ErrorCode errorCode)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }

        public int? StatusCode { get; }

        public ErrorCode ErrorCode { get; }
    }
}
