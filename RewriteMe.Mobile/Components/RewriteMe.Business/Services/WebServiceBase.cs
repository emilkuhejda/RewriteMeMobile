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
        protected WebServiceBase(
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
        {
            WebServiceErrorHandler = webServiceErrorHandler;
            ApplicationSettings = applicationSettings;
        }

        protected IWebServiceErrorHandler WebServiceErrorHandler { get; }

        protected IApplicationSettings ApplicationSettings { get; }

        protected async Task<T> MakeServiceCall<T>(Func<RewriteMeClient, Task<T>> webServiceCall, CustomHeadersDictionary customHeaders = null, int timeoutSeconds = 600)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true
            };

            var httpClient = new HttpClient(httpClientHandler) { Timeout = TimeSpan.FromSeconds(timeoutSeconds) };

            try
            {
                var rewriteMeClient = new RewriteMeClient(ApplicationSettings.WebApiUrl, httpClient);

                if (customHeaders != null)
                {
                    rewriteMeClient.AddCustomHeaders(customHeaders);
                }

                return await webServiceCall(rewriteMeClient).ConfigureAwait(false);
            }
            finally
            {
                httpClientHandler.Dispose();
                httpClient.Dispose();
            }
        }
    }
}
