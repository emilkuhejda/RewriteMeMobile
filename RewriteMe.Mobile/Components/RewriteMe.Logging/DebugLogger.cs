using System;
using System.Diagnostics;
using RewriteMe.Logging.Enums;

namespace RewriteMe.Logging
{
    public class DebugLogger : LoggerBase
    {
        public DebugLogger(string name)
        {
            Name = name;
        }

        public override string Name { get; }

        protected override void WriteCore(LogEntry entry)
        {
            var message = entry.Exception == null
                ? $"{DateTime.UtcNow} - {entry.Category} - {Name} - {entry.Message} [EOL]"
                : $"{DateTime.UtcNow} - {entry.Category} - {Name} - {entry.Message} - Exception: {entry.Exception} [EOL]";

            Debug.WriteLine($"{message}");
        }

        public override bool IsCategoryEnabled(Category category)
        {
            return true;
        }
    }
}
