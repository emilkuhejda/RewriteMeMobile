using Prism.Ioc;
using RewriteMe.Common;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business
{
    public class BusinessModule : IUnityModule
    {
        public void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IInternalValueService, IInternalValueService>();
        }
    }
}
