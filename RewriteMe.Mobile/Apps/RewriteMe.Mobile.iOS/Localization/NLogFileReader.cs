using System.IO;
using System.Threading.Tasks;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Mobile.iOS.Localization
{
    public class NLogFileReader : ILogFileReader
    {
        private readonly ILoggerConfiguration _loggerConfiguration;

        public NLogFileReader(ILoggerConfiguration loggerConfiguration)
        {
            _loggerConfiguration = loggerConfiguration;
        }

        public async Task<string> ReadLogFileAsync()
        {
            var logFile = GetLogFileInfo();
            if (!logFile.Exists)
            {
                throw new FileNotFoundException(logFile.FullName);
            }

            using (var streamReader = new StreamReader(logFile.FullName))
            {
                var content = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                return content;
            }
        }

        public async Task ClearLogFileAsync()
        {
            var logFile = GetLogFileInfo();
            if (!logFile.Exists)
            {
                throw new FileNotFoundException(logFile.FullName);
            }

            using (var streamWriter = new StreamWriter(logFile.FullName))
            {
                await streamWriter.FlushAsync().ConfigureAwait(false);
            }
        }

        public FileInfo GetLogFileInfo()
        {
            return _loggerConfiguration.GetLogFileInfo();
        }
    }
}
