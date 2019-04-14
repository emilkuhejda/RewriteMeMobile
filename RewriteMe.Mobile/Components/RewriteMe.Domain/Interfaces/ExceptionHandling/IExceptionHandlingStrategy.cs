using System;

namespace RewriteMe.Domain.Interfaces.ExceptionHandling
{
    public interface IExceptionHandlingStrategy
    {
        bool HandleException(Exception exception);
    }
}
