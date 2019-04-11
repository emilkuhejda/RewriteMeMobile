using System;
using System.Net.Http;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Factories
{
    public class RewriteMeApiClient : RewritemeAPI, IApiClient
    {
        public RewriteMeApiClient(Uri baseUri)
            : base(baseUri)
        {
        }

        public HttpClientHandler HttpHandler => HttpClientHandler;
    }
}
