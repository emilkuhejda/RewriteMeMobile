using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Configuration;
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

        public async Task<bool> IsAliveAsync()
        {
            var timeout = TimeSpan.FromSeconds(5);
            var client = RewriteMeApiClientFactory.CreateSingleClient(ApplicationSettings.WebApiUri, timeout);

            var httpRequestResult = await WebServiceErrorHandler.HandleResponseAsync(() => client.IsAliveWithHttpMessagesAsync()).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                var result = httpRequestResult.Payload?.Body;
                return result.HasValue && result.Value;
            }

            return false;
        }

        public async Task<HttpRequestResult<LastUpdates>> GetLastUpdatesAsync()
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetLastUpdatesAsync(customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<FileItem>>> GetFileItemsAsync(DateTime updatedAfter)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetFileItemsAsync(updatedAfter, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<Guid>>> GetDeletedFileItemIdsAsync(DateTime updatedAfter)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetDeletedFileItemIdsAsync(updatedAfter, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<TimeSpanWrapper>> GetDeletedFileItemsTotalTimeAsync()
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetDeletedFileItemsTotalTimeAsync(customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<TimeSpanWrapper>> DeleteFileItemAsync(Guid fileItemId)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.DeleteFileItemAsync(fileItemId, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> DeleteAllFileItemsAsync(IList<DeletedFileItem> fileItems)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.DeleteAllFileItemsAsync(fileItems, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<TranscribeItem>>> GetTranscribeItemsAllAsync(DateTime updatedAfter)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetTranscribeItemsAllAsync(updatedAfter, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<UserSubscription>>> GetUserSubscriptionsAsync(DateTime updatedAfter)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetUserSubscriptionsAsync(updatedAfter, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<UserSubscription>> CreateUserSubscriptionAsync(BillingPurchase billingPurchase)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.CreateUserSubscriptionAsync(billingPurchase, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<SpeechConfiguration>> GetSpeechConfigurationAsync()
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetSpeechConfigurationAsync(customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<FileItem>> UploadFileItemAsync(MediaFile mediaFile, CancellationToken cancellationToken)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.UploadFileItemAsync(mediaFile, DateTime.Now, ApplicationSettings.ApplicationId, customHeaders, cancellationToken)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> TranscribeFileItemAsync(Guid fileItemId, string language)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.TranscribeFileItemAsync(fileItemId, language, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<byte[]>> GetTranscribeAudioSourceAsync(Guid transcribeItemId, CancellationToken cancellationToken)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetTranscribeAudioSourceAsync(transcribeItemId, customHeaders, cancellationToken)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> UpdateUserTranscriptAsync(Guid transcribeItemId, string transcript)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.UpdateUserTranscriptAsync(transcribeItemId, transcript, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> CreateSpeechResultAsync(Guid speechResultId, Guid recognizedAudioSampleId, string displayText)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.CreateSpeechResultAsync(speechResultId, recognizedAudioSampleId, displayText, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> UpdateSpeechResultsAsync(IList<SpeechResultModel> speechResults)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.UpdateSpeechResultsAsync(speechResults, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<InformationMessage>>> GetInformationMessagesAsync(DateTime updatedAfter)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetInformationMessagesAsync(updatedAfter, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<InformationMessage>> MarkMessageAsOpenedAsync(Guid informationMessageId)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.MarkMessageAsOpenedAsync(informationMessageId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> MarkMessagesAsOpenedAsync(IEnumerable<Guid?> ids)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.MarkMessagesAsOpenedAsync(ids, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<RecognizedTime>> GetRecognizedTimeAsync()
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetRecognizedTimeAsync(customHeaders)).ConfigureAwait(false);
        }

        public async Task RefreshTokenIfNeededAsync()
        {
            var accessToken = _userSessionService.AccessToken;
            var daysToExpire = accessToken.ExpirationDate.Subtract(DateTimeOffset.UtcNow).TotalDays;
            if (daysToExpire > 30)
                return;

            var customHeaders = GetAuthHeaders();
            var httpRequestResult = await WebServiceErrorHandler.HandleResponseAsync(() => Client.RefreshTokenAsync(customHeaders)).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                _userSessionService.SetToken(httpRequestResult.Payload);
            }
        }

        private CustomHeadersDictionary GetAuthHeaders()
        {
            var accessToken = _userSessionService.GetToken();
            return new CustomHeadersDictionary().AddBearerToken(accessToken);
        }
    }
}
