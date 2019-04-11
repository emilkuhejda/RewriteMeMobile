using System.Net.Http;

namespace RewriteMe.Domain.Interfaces.Factories
{
    public interface IApiClient
    {
        HttpClient HttpClient { get; }

        HttpClientHandler HttpHandler { get; }
    }
}
