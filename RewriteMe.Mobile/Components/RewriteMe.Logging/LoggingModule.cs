using Prism.Ioc;
using RewriteMe.Common;
using RewriteMe.Logging.Debug;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Logging
{
    public class LoggingModule : IUnityModule
    {
        public void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILoggerFactory, DebugLoggerFactory>();
        }
    }
}
