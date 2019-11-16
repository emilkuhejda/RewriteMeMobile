using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Services
{
    public abstract class WebServiceBase
    {
        private IVoicipherAPI _client;

        protected WebServiceBase(
            IRewriteMeApiClientFactory rewriteMeApiClientFactory,
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
        {
            RewriteMeApiClientFactory = rewriteMeApiClientFactory;
            WebServiceErrorHandler = webServiceErrorHandler;
            ApplicationSettings = applicationSettings;
        }

        protected IVoicipherAPI Client => _client ?? (_client = RewriteMeApiClientFactory.CreateClient(ApplicationSettings.WebApiUri));

        protected IRewriteMeApiClientFactory RewriteMeApiClientFactory { get; }

        protected IWebServiceErrorHandler WebServiceErrorHandler { get; }

        protected IApplicationSettings ApplicationSettings { get; }
    }
}
