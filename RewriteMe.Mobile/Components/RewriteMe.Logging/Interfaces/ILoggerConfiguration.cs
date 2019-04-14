using System.IO;

namespace RewriteMe.Logging.Interfaces
{
    public interface ILoggerConfiguration
    {
        void Initialize(string logFileName);

        FileInfo GetLogFilePath();
    }
}
