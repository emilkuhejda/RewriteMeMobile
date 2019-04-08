using Prism.Ioc;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Mobile.Configuration;
using RewriteMe.Mobile.iOS.Localization;

namespace RewriteMe.Mobile.iOS.Configuration
{
    public class OSXBootstrapper : Bootstrapper
    {
        protected override void RegisterPlatformServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILocalizer, Localizer>();
        }
    }
}