using System;
using System.Diagnostics;
using RewriteMe.Logging.Enums;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Logging.Extensions
{
    public static class LoggerExtensions
    {
        [DebuggerStepThrough]
        public static void Debug(this ILogger logger, string message)
        {
            logger.Write(Category.Debug, message);
        }

        [DebuggerStepThrough]
        public static void Info(this ILogger logger, string message)
        {
            logger.Write(Category.Info, message);
        }

        [DebuggerStepThrough]
        public static void Warning(this ILogger logger, string message)
        {
            logger.Write(Category.Warning, message);
        }

        [DebuggerStepThrough]
        public static void Error(this ILogger logger, string message)
        {
            logger.Write(Category.Error, message);
        }

        [DebuggerStepThrough]
        public static void Exception(this ILogger logger, Exception exception, string message = "")
        {
            logger.Write(Category.Error, exception, message);
        }

        [DebuggerStepThrough]
        public static void Fatal(this ILogger logger, Exception exception, string message)
        {
            logger.Write(Category.Fatal, exception, message);
        }
    }
}
