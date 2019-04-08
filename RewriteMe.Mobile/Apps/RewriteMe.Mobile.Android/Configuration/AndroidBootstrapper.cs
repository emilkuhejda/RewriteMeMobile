using Prism.Ioc;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Mobile.Configuration;
using RewriteMe.Mobile.Droid.Localization;

namespace RewriteMe.Mobile.Droid.Configuration
{
    public class AndroidBootstrapper : Bootstrapper
    {
        protected override void RegisterPlatformServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ILocalizer, Localizer>();
        }
    }
}