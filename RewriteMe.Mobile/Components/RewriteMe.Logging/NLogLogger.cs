using System;
using NLog;
using RewriteMe.Logging.Enums;

namespace RewriteMe.Logging
{
    public class NLogLogger : LoggerBase
    {
        private readonly Logger _logger;

        public NLogLogger(string loggerName)
        {
            _logger = LogManager.GetLogger(loggerName);
        }

        public override string Name => _logger.Name;

        protected override void WriteCore(LogEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            _logger.Log(CategoryToLogLevel(entry.Category), entry.Exception, entry.Message);
        }

        public override bool IsCategoryEnabled(Category category)
        {
            return _logger.IsEnabled(CategoryToLogLevel(category));
        }

        private static LogLevel CategoryToLogLevel(Category category)
        {
            switch (category)
            {
                case Category.Debug:
                    return LogLevel.Debug;
                case Category.Info:
                    return LogLevel.Info;
                case Category.Warning:
                    return LogLevel.Warn;
                case Category.Error:
                    return LogLevel.Error;
                case Category.Fatal:
                    return LogLevel.Fatal;
                default:
                    throw new InvalidOperationException("Mapping of Category.{0} to NLog LogLevel.{1} not possible.");
            }
        }
    }
}
