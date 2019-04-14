using NLog.Layouts;
using NLog.Targets;

namespace RewriteMe.Mobile.iOS.Localization
{
    internal static partial class NLogTargets
    {
        internal static Target GetConsoletTarget(Layout layout)
        {
            var consoleTarget = new ConsoleTarget();
            consoleTarget.Layout = layout;
            return consoleTarget;
        }
    }
}