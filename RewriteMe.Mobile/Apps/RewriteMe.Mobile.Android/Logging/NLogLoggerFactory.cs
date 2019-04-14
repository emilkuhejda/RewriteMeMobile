using System;
using RewriteMe.Logging;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Mobile.Droid.Logging
{
    public class NLogLoggerFactory : ILoggerFactory
    {
        private const string LogFileName = "Uster.MobileAlerts.Mobile.log";

        public NLogLoggerFactory(ILoggerConfiguration loggerConfiguration)
        {
            if (loggerConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerConfiguration));
            }

            loggerConfiguration.Initialize(LogFileName);
        }

        public ILogger CreateLogger(Type dependantType)
        {
            if (dependantType == null)
            {
                throw new ArgumentNullException(nameof(dependantType));
            }

            return new NLogLogger(dependantType.Name);
        }
    }
}