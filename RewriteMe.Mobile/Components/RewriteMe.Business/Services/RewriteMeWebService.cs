using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.Transcription;
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

        public async Task<HttpRequestResult<IEnumerable<FileItem>>> GetFileItemsAsync(DateTime updatedAfter)
        {
            var customHeaders = await GetAuthHeaders().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetFileItemsAsync(updatedAfter, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<TranscribeItem>>> GetTranscribeItemsAllAsync(DateTime updatedAfter)
        {
            var customHeaders = await GetAuthHeaders().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetTranscribeItemsAllAsync(updatedAfter, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<UserSubscription>>> GetUserSubscriptionsAsync(DateTime updatedAfter)
        {
            var customHeaders = await GetAuthHeaders().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetUserSubscriptionsAsync(updatedAfter, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<SubscriptionProduct>>> GetSubscriptionProductsAsync()
        {
            var customHeaders = await GetAuthHeaders().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetSubscriptionProductsAsync(customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<FileItem>> UploadFileItemAsync(MediaFile mediaFile)
        {
            var customHeaders = await GetAuthHeaders().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.UploadFileItemAsync(mediaFile, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> TranscribeFileItemAsync(Guid fileItemId, string language)
        {
            var customHeaders = await GetAuthHeaders().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.TranscribeFileItemAsync(fileItemId, language, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<byte[]>> GetTranscribeAudioSourceAsync(Guid transcribeItemId)
        {
            var customHeaders = await GetAuthHeaders().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetTranscribeAudioSourceAsync(transcribeItemId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> UpdateUserTranscriptAsync(Guid transcribeItemId, string transcript)
        {
            var customHeaders = await GetAuthHeaders().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.UpdateUserTranscriptAsync(transcribeItemId, transcript, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        private async Task<CustomHeadersDictionary> GetAuthHeaders()
        {
            var accessToken = await _userSessionService.GetAccessTokenSilentAsync().ConfigureAwait(false);
            return new CustomHeadersDictionary().AddBearerToken(accessToken);
        }
    }
}
