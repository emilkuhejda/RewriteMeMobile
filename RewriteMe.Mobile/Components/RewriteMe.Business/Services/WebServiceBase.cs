using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Services
{
    public abstract class WebServiceBase
    {
        private HttpClient _client;

        protected WebServiceBase(
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
        {
            WebServiceErrorHandler = webServiceErrorHandler;
            ApplicationSettings = applicationSettings;
        }

        private HttpClient Client => _client ?? (_client = CreateHttpClient());

        protected IWebServiceErrorHandler WebServiceErrorHandler { get; }

        protected IApplicationSettings ApplicationSettings { get; }

        protected async Task<T> MakeServiceCall<T>(Func<RewriteMeClient, Task<T>> webServiceCall, CustomHeadersDictionary customHeaders = null)
        {
            var rewriteMeClient = new RewriteMeClient(ApplicationSettings.WebApiUrl, Client);

            if (customHeaders != null)
            {
                rewriteMeClient.AddCustomHeaders(customHeaders);
            }

            return await webServiceCall(rewriteMeClient).ConfigureAwait(false);
        }

        protected HttpClient CreateHttpClient(int timeoutSeconds = 600)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            };

            var httpClient = new HttpClient(httpClientHandler)
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };

            return httpClient;
        }
    }
}
