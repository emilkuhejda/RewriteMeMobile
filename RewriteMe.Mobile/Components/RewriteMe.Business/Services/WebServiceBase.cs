using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Services
{
    public abstract class WebServiceBase
    {
        private readonly IRewriteMeApiClientFactory _rewriteMeApiClientFactory;

        private IRewriteMeAPI _client;

        protected WebServiceBase(
            IRewriteMeApiClientFactory rewriteMeApiClientFactory,
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
        {
            _rewriteMeApiClientFactory = rewriteMeApiClientFactory;

            WebServiceErrorHandler = webServiceErrorHandler;
            ApplicationSettings = applicationSettings;
        }

        protected IRewriteMeAPI Client => _client ?? (_client = _rewriteMeApiClientFactory.CreateClient(ApplicationSettings.WebApiUri));

        protected IWebServiceErrorHandler WebServiceErrorHandler { get; }

        protected IApplicationSettings ApplicationSettings { get; }
    }
}
