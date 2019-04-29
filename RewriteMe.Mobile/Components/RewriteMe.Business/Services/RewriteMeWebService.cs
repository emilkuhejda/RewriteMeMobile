using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Services
{
    public class RewriteMeWebService : WebServiceBase, IRewriteMeWebService
    {
        private readonly IUserSessionService _userSessionService;

        public RewriteMeWebService(
            IUserSessionService userSessionService,
            IRewriteMeApiClientFactory rewriteMeApiClientFactory,
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
            : base(rewriteMeApiClientFactory, webServiceErrorHandler, applicationSettings)
        {
            _userSessionService = userSessionService;
        }

        public async Task<HttpRequestResult<LastUpdates>> GetLastUpdates()
        {
            var customHeaders = await GetAuthHeaders().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetLastUpdates(customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<FileItem>>> GetFileItemsAsync(int? minimumVersion = 0)
        {
            var customHeaders = await GetAuthHeaders().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetFileItemsAsync(0, customHeaders)).ConfigureAwait(false);
        }

        private async Task<CustomHeadersDictionary> GetAuthHeaders()
        {
            var accessToken = await _userSessionService.GetAccessTokenSilentAsync().ConfigureAwait(false);
            return new CustomHeadersDictionary().AddBearerToken(accessToken);
        }
    }
}
