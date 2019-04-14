﻿using System;
using System.IO;
using NLog;
using NLog.Config;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Logging.Layouts;

namespace RewriteMe.Mobile.Droid.Logging
{
    public class NLogLoggerConfiguration : ILoggerConfiguration
    {
        private string _fileName;

        public void Initialize(string logFileName)
        {
            _fileName = logFileName;

            var config = new LoggingConfiguration();
            var layout = NLogLayouts.GetDefaultLayout();

            // Console Target
            var consoleTarget = NLogTargets.GetConsoletTarget(layout);
            config.AddTarget("console", consoleTarget);

            var consoleRule = new LoggingRule("*", LogLevel.Trace, consoleTarget);
            config.LoggingRules.Add(consoleRule);

            // File Target
            var logFilePath = GetLogFilePath().FullName;
            var fileTarget = NLogTargets.GetFileTarget(layout, logFilePath);
            config.AddTarget("file", fileTarget);

            var fileRule = new LoggingRule("*", LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(fileRule);

            LogManager.Configuration = config;
        }

        public FileInfo GetLogFilePath()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(folder))
            {
                folder = Directory.CreateDirectory(folder).FullName;
            }

            var filePath = Path.Combine(folder, _fileName);
            return new FileInfo(filePath);
        }
    }
}
