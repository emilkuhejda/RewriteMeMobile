using System;
using RewriteMe.Domain.Interfaces.ExceptionHandling;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Mobile.ExceptionHandling
{
    public class ExceptionHandlingStrategy : IExceptionHandlingStrategy
    {
        private readonly ILogger _logger;

        public ExceptionHandlingStrategy(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(ExceptionHandlingStrategy));
        }

        public bool HandleException(Exception exception)
        {
            _logger.Exception(exception);

            return false;
        }
    }
}
