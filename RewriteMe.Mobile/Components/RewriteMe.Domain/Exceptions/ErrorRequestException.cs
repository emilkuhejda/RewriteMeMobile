using System;

namespace RewriteMe.Domain.Exceptions
{
    public class ErrorRequestException : Exception
    {
        public ErrorRequestException(int? statusCode)
        {
            StatusCode = statusCode;
        }

        public int? StatusCode { get; }
    }
}
