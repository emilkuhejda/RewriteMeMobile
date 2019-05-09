using System;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Exceptions
{
    public class ProblemDetailsException : Exception
    {
        public ProblemDetailsException()
        {
        }

        public ProblemDetailsException(string message)
            : base(message)
        {
        }

        public ProblemDetailsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ProblemDetailsException(ProblemDetails problemDetails)
        {
            ProblemDetails = problemDetails;
        }

        public int? StatusCode => ProblemDetails.Status;

        public ProblemDetails ProblemDetails { get; }
    }
}
