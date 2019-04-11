using System;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Services
{
    public class RewriteMeWebService : IRewriteMeWebService
    {
        private readonly IRewriteMeApiClientFactory _rewriteMeApiClientFactory;
        private readonly IWebServiceErrorHandler _webServiceErrorHandler;

        private IRewriteMeAPI _client;

        public RewriteMeWebService(
            IRewriteMeApiClientFactory rewriteMeApiClientFactory,
            IWebServiceErrorHandler webServiceErrorHandler)
        {
            _rewriteMeApiClientFactory = rewriteMeApiClientFactory;
            _webServiceErrorHandler = webServiceErrorHandler;
        }

        private IRewriteMeAPI Client => _client ?? (_client = _rewriteMeApiClientFactory.CreateClient(new Uri(string.Empty)));
    }
}
