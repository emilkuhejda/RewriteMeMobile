using System;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Interfaces.Factories;

namespace RewriteMe.Business.Factories
{
    public class RewriteMeApiClientFactory : IRewriteMeApiClientFactory
    {
        private RewriteMeApiClient _client;

        public RewriteMeApiClient CreateClient(Uri baseUri)
        {
            if (_client != null)
                return _client;

            var client = new RewriteMeApiClient(baseUri);
            client.Optimize();

            return _client = client;
        }

        public RewriteMeApiClient CreateSingleClient(Uri baseUri, TimeSpan timeout)
        {
            var client = new RewriteMeApiClient(baseUri);
            client.Optimize(timeout);

            return client;
        }
    }
}
