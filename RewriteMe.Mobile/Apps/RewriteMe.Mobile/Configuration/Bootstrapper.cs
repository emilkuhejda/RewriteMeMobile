using Prism;
using Prism.Ioc;
using Prism.Unity;
using RewriteMe.Business;
using RewriteMe.DataAccess;
using RewriteMe.Domain;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Mobile.Localization;
using Unity;

namespace RewriteMe.Mobile.Configuration
{
    public abstract class Bootstrapper : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var mobileModule = new MobileModule();
            mobileModule.RegisterServices(containerRegistry);

            var businessModule = new BusinessModule();
            businessModule.RegisterServices(containerRegistry);

            var dataAccessMobule = new DataAccessMobule();
            dataAccessMobule.RegisterServices(containerRegistry);

            var domainModule = new DomainModule();
            domainModule.RegisterServices(containerRegistry);

            RegisterPlatformServices(containerRegistry);
            InitializeServices(containerRegistry);
        }

        protected abstract void RegisterPlatformServices(IContainerRegistry containerRegistry);

        private void InitializeServices(IContainerRegistry containerRegistry)
        {
            LocalizationExtension.Init(() => containerRegistry.GetContainer().Resolve<ILocalizer>());
        }
    }
}
