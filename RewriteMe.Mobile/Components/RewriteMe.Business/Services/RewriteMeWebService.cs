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
using RewriteMe.Domain.WebApi;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class RewriteMeWebService : WebServiceBase, IRewriteMeWebService
    {
        private readonly IUserSessionService _userSessionService;
        private readonly ILogger _logger;

        public RewriteMeWebService(
            IUserSessionService userSessionService,
            ILoggerFactory loggerFactory,
            IRewriteMeApiClientFactory rewriteMeApiClientFactory,
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
            : base(rewriteMeApiClientFactory, webServiceErrorHandler, applicationSettings)
        {
            _userSessionService = userSessionService;
            _logger = loggerFactory.CreateLogger(typeof(RewriteMeWebService));
        }

        public async Task<bool> IsAliveAsync()
        {
            var timeout = TimeSpan.FromSeconds(5);

            using (var client = RewriteMeApiClientFactory.CreateSingleClient(ApplicationSettings.WebApiUri, timeout))
            {
                try
                {
                    var result = await client.IsAliveAsync(ApplicationSettings.WebApiVersion).ConfigureAwait(false);
                    return result.HasValue && result.Value;
                }
                catch (Exception exception)
                {
                    var message = "Exception during 'is-alive' web service request.";
                    _logger.Warning($"{message} {exception}");

                    return false;
                }

            }
        }

        public async Task<HttpRequestResult<LastUpdates>> GetLastUpdatesAsync()
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetLastUpdatesAsync(ApplicationSettings.WebApiVersion, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<FileItem>>> GetFileItemsAsync(DateTime updatedAfter)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetFileItemsAsync(ApplicationSettings.WebApiVersion, updatedAfter, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<Guid>>> GetDeletedFileItemIdsAsync(DateTime updatedAfter)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetDeletedFileItemIdsAsync(ApplicationSettings.WebApiVersion, updatedAfter, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> DeleteFileItemAsync(Guid fileItemId)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.DeleteFileItemAsync(ApplicationSettings.WebApiVersion, fileItemId, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> DeleteAllFileItemsAsync(IList<DeletedFileItem> fileItems)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.DeleteAllFileItemsAsync(ApplicationSettings.WebApiVersion, fileItems, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<TranscribeItem>>> GetTranscribeItemsAllAsync(DateTime updatedAfter)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetTranscribeItemsAllAsync(ApplicationSettings.WebApiVersion, updatedAfter, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<TimeSpanWrapper>> GetUserSubscriptionRemainingTimeAsync()
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetUserSubscriptionRemainingTimeAsync(ApplicationSettings.WebApiVersion, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<TimeSpanWrapper>> CreateUserSubscriptionAsync(BillingPurchase billingPurchase)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.CreateUserSubscriptionAsync(ApplicationSettings.WebApiVersion, billingPurchase, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<SpeechConfiguration>> GetSpeechConfigurationAsync()
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetSpeechConfigurationAsync(ApplicationSettings.WebApiVersion, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<FileItem>> UploadFileItemAsync(MediaFile mediaFile, CancellationToken cancellationToken)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.UploadFileItemAsync(ApplicationSettings.WebApiVersion, mediaFile, DateTime.Now, ApplicationSettings.ApplicationId, customHeaders, cancellationToken)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> TranscribeFileItemAsync(Guid fileItemId, string language)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.TranscribeFileItemAsync(ApplicationSettings.WebApiVersion, fileItemId, language, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<byte[]>> GetTranscribeAudioSourceAsync(Guid transcribeItemId, CancellationToken cancellationToken)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetTranscribeAudioSourceAsync(ApplicationSettings.WebApiVersion, transcribeItemId, customHeaders, cancellationToken)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> UpdateUserTranscriptAsync(Guid transcribeItemId, string transcript)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.UpdateUserTranscriptAsync(ApplicationSettings.WebApiVersion, transcribeItemId, transcript, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> CreateSpeechResultAsync(Guid speechResultId, Guid recognizedAudioSampleId, string displayText)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.CreateSpeechResultAsync(ApplicationSettings.WebApiVersion, speechResultId, recognizedAudioSampleId, displayText, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<TimeSpanWrapper>> UpdateSpeechResultsAsync(IList<SpeechResultModel> speechResults)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.UpdateSpeechResultsAsync(ApplicationSettings.WebApiVersion, speechResults, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<InformationMessage>>> GetInformationMessagesAsync(DateTime updatedAfter)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.GetInformationMessagesAsync(ApplicationSettings.WebApiVersion, updatedAfter, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<InformationMessage>> MarkMessageAsOpenedAsync(Guid informationMessageId)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.MarkMessageAsOpenedAsync(ApplicationSettings.WebApiVersion, informationMessageId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> MarkMessagesAsOpenedAsync(IEnumerable<Guid?> ids)
        {
            var customHeaders = GetAuthHeaders();
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.MarkMessagesAsOpenedAsync(ApplicationSettings.WebApiVersion, ids, customHeaders)).ConfigureAwait(false);
        }

        public async Task RefreshTokenIfNeededAsync()
        {
            var accessToken = _userSessionService.AccessToken;
            var daysToExpire = accessToken.ExpirationDate.Subtract(DateTimeOffset.UtcNow).TotalDays;
            if (daysToExpire > 30)
                return;

            var customHeaders = GetAuthHeaders();
            var httpRequestResult = await WebServiceErrorHandler.HandleResponseAsync(() => Client.RefreshTokenAsync(ApplicationSettings.WebApiVersion, customHeaders)).ConfigureAwait(false);
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
