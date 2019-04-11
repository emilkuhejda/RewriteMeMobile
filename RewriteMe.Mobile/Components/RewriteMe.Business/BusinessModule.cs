using Prism.Ioc;
using RewriteMe.Business.Factories;
using RewriteMe.Business.Services;
using RewriteMe.Business.Utils;
using RewriteMe.Common;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;

namespace RewriteMe.Business
{
    public class BusinessModule : IUnityModule
    {
        public void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IInternalValueService, IInternalValueService>();
            containerRegistry.RegisterSingleton<IRewriteMeWebService, RewriteMeWebService>();

            containerRegistry.RegisterSingleton<IRewriteMeApiClientFactory, RewriteMeApiClientFactory>();

            containerRegistry.RegisterSingleton<IWebServiceErrorHandler, WebServiceErrorHandler>();
        }
    }
}
