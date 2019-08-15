using System;
using RewriteMe.Domain.Interfaces.ExceptionHandling;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Mobile.ExceptionHandling
{
    public class ExceptionHandlingStrategy : IExceptionHandlingStrategy
    {
        private readonly IAppCenterMetricsService _appCenterMetricsService;
        private readonly ILogger _logger;

        public ExceptionHandlingStrategy(
            IAppCenterMetricsService appCenterMetricsService,
            ILoggerFactory loggerFactory)
        {
            _appCenterMetricsService = appCenterMetricsService;
            _logger = loggerFactory.CreateLogger(typeof(ExceptionHandlingStrategy));
        }

        public bool HandleException(Exception exception)
        {
            _logger.Exception(exception);
            _appCenterMetricsService.TrackException(exception);

            return false;
        }
    }
}
