using Prism;
using Prism.Ioc;
using RewriteMe.Business;
using RewriteMe.DataAccess;
using RewriteMe.Domain;

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
        }

        protected abstract void RegisterPlatformServices(IContainerRegistry containerRegistry);
    }
}
