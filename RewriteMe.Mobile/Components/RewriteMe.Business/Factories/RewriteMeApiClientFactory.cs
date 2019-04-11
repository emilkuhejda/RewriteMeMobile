using System;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Interfaces.Factories;

namespace RewriteMe.Business.Factories
{
    public class RewriteMeApiClientFactory : IRewriteMeApiClientFactory
    {
        public RewriteMeApiClient CreateClient(Uri baseUri)
        {
            var client = new RewriteMeApiClient(baseUri);
            client.Optimize();

            return client;
        }
    }
}
