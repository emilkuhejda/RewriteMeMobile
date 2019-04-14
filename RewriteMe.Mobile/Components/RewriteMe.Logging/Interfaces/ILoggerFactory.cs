using System;

namespace RewriteMe.Logging.Interfaces
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger(Type dependantType);
    }
}
