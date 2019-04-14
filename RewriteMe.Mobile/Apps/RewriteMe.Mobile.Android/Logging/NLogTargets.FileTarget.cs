using NLog.Layouts;
using NLog.Targets;

namespace RewriteMe.Mobile.Droid.Logging
{
    internal static partial class NLogTargets
    {
        internal static Target GetFileTarget(Layout layout, string logFilePath)
        {
            var fileTarget = new FileTarget();
            fileTarget.FileName = logFilePath;
            fileTarget.Layout = layout;
            fileTarget.ArchiveFileName = logFilePath + ".{#####}.log";
            fileTarget.ArchiveAboveSize = 1000000; // 1MB
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.Sequence;
            fileTarget.MaxArchiveFiles = 1;
            fileTarget.ConcurrentWrites = true;
            fileTarget.KeepFileOpen = false;
            return fileTarget;
        }
    }
}