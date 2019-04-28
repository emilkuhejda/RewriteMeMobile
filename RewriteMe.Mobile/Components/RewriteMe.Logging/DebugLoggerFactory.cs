using System;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Logging
{
    public class DebugLoggerFactory : ILoggerFactory
    {
        public ILogger CreateLogger(Type dependantType)
        {
            return new DebugLogger(dependantType.Name);
        }
    }
}
