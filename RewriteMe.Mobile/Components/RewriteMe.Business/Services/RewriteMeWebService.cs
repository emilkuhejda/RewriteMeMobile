using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi;
using RewriteMe.Domain.WebApi.Models;

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

        private IRewriteMeAPI Client => _client ?? (_client = _rewriteMeApiClientFactory.CreateClient(new Uri("https://192.168.0.105:45456/")));

        public async Task<HttpRequestResult<object>> RegisterUserAsync(RegisterUserModel registerUserModel)
        {
            return await _webServiceErrorHandler.HandleResponseAsync(() => Client.RegisterUserAsync(registerUserModel));
        }
    }
}
