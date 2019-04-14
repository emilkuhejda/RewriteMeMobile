using System;
using RewriteMe.Logging.Enums;

namespace RewriteMe.Logging
{
    public class LogEntry
    {
        public LogEntry(Category category, string message)
            : this(category, null, message)
        {
        }

        public LogEntry(Category category, Exception exception, string message)
        {
            Category = category;
            Exception = exception;
            Message = message;
        }

        public Category Category { get; }

        public Exception Exception { get; }

        public string Message { get; }
    }
}
