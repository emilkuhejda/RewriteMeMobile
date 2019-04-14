using System;
using RewriteMe.Logging.Enums;

namespace RewriteMe.Logging.Interfaces
{
    public interface ILogger
    {
        string Name { get; }

        void Write(Category category, string message);

        void Write(Category category, Exception exception, string message);
    }
}
