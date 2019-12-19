using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Services
{
    public abstract class WebServiceBase
    {
        protected WebServiceBase(
            IUserSessionService userSessionService,
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
        {
            UserSessionService = userSessionService;
            WebServiceErrorHandler = webServiceErrorHandler;
            ApplicationSettings = applicationSettings;
        }

        protected IUserSessionService UserSessionService { get; }

        protected IWebServiceErrorHandler WebServiceErrorHandler { get; }

        protected IApplicationSettings ApplicationSettings { get; }

        protected async Task<T> MakeServiceCall<T>(Func<RewriteMeClient, Task<T>> webServiceCall, int timeoutSeconds = 600)
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
                rewriteMeClient.AddCustomHeaders(GetAuthHeaders());

                return await webServiceCall(rewriteMeClient).ConfigureAwait(false);
            }
            finally
            {
                httpClientHandler.Dispose();
                httpClient.Dispose();
            }
        }

        private CustomHeadersDictionary GetAuthHeaders()
        {
            var accessToken = UserSessionService.GetToken();
            return new CustomHeadersDictionary().AddBearerToken(accessToken);
        }
    }
}
