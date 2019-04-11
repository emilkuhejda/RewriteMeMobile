using System;

namespace RewriteMe.Domain.Interfaces.Factories
{
    public interface IRewriteMeApiClientFactory
    {
        RewriteMeApiClient CreateClient(Uri baseUri);
    }
}
