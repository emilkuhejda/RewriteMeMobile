using System.Diagnostics;
using NLog;
using NLog.Layouts;
using NLog.Targets;

namespace RewriteMe.Mobile.iOS.Localization
{
    internal static partial class NLogTargets
    {
        internal static Target GetDebugTarget(Layout layout)
        {
            var debugTarget = new DebugTarget();
            debugTarget.Layout = layout;
            return debugTarget;
        }

        internal class DebugTarget : TargetWithLayout
        {
            protected override void Write(LogEventInfo logEvent)
            {
                var logMessage = Layout.Render(logEvent);

                Debug.WriteLine(logMessage);
            }
        }
    }
}