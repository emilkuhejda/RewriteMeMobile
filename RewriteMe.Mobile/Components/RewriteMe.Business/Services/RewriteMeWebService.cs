using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Extensions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi;
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
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
            : base(webServiceErrorHandler, applicationSettings)
        {
            _userSessionService = userSessionService;
            _logger = loggerFactory.CreateLogger(typeof(RewriteMeWebService));
        }

        public async Task<bool> IsAliveAsync()
        {
            try
            {
                return await MakeServiceCall(client => client.IsAliveAsync(ApplicationSettings.WebApiVersion), timeoutSeconds: 5).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                var message = "Exception during 'is-alive' web service request.";
                _logger.Warning($"{message} {exception}");

                return false;
            }
        }

        public async Task<HttpRequestResult<LastUpdates>> GetLastUpdatesAsync()
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                    () => MakeServiceCall(client => client.GetLastUpdatesAsync(ApplicationSettings.WebApiVersion), GetAuthHeaders())
                    ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<FileItem>>> GetFileItemsAsync(DateTime updatedAfter)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.GetFileItemsAsync(updatedAfter, ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<Guid>>> GetDeletedFileItemIdsAsync(DateTime updatedAfter)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.GetDeletedFileItemIdsAsync(updatedAfter, ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> DeleteFileItemAsync(Guid fileItemId)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.DeleteFileItemAsync(fileItemId, ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> DeleteAllFileItemsAsync(IList<DeletedFileItem> fileItems)
        {
            var deletedFileItemModels = fileItems.Select(x => x.ToDeletedFileItemModel());
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.DeleteAllFileItemsAsync(ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion, deletedFileItemModels), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<TranscribeItem>>> GetTranscribeItemsAllAsync(DateTime updatedAfter)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.GetTranscribeItemsAllAsync(updatedAfter, ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<TimeSpanWrapper>> GetUserSubscriptionRemainingTimeAsync()
        {
            throw new NotImplementedException();
            //return await WebServiceErrorHandler.HandleResponseAsync(client => client.GetUserSubscriptionRemainingTimeAsync(ApplicationSettings.WebApiVersion, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<TimeSpanWrapper>> CreateUserSubscriptionAsync(BillingPurchase billingPurchase)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.CreateUserSubscriptionAsync(ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion, billingPurchase), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<SpeechConfiguration>> GetSpeechConfigurationAsync()
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.GetSpeechConfigurationAsync(ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<FileItem>> UploadFileItemAsync(MediaFile mediaFile, CancellationToken cancellationToken)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client =>
                {
                    using (var stream = new MemoryStream(mediaFile.Source))
                    {
                        return client.UploadFileItemAsync(mediaFile.Name, mediaFile.Language, mediaFile.FileName, DateTime.Now, ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion, stream, cancellationToken);
                    }
                }, GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> TranscribeFileItemAsync(Guid fileItemId, string language)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.TranscribeFileItemAsync(fileItemId, language, ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<byte[]>> GetTranscribeAudioSourceAsync(Guid transcribeItemId, CancellationToken cancellationToken)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.GetTranscribeAudioSourceAsync(transcribeItemId, ApplicationSettings.WebApiVersion, cancellationToken), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> UpdateUserTranscriptAsync(Guid transcribeItemId, string transcript)
        {
            throw new NotImplementedException();
            //return await WebServiceErrorHandler.HandleResponseAsync(client => client.UpdateUserTranscriptAsync(ApplicationSettings.WebApiVersion, transcribeItemId, transcript, ApplicationSettings.ApplicationId, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> CreateSpeechResultAsync(Guid speechResultId, Guid recognizedAudioSampleId, string displayText)
        {
            throw new NotImplementedException();
            //return await WebServiceErrorHandler.HandleResponseAsync(client => client.CreateSpeechResultAsync(ApplicationSettings.WebApiVersion, speechResultId, recognizedAudioSampleId, displayText, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<TimeSpanWrapper>> UpdateSpeechResultsAsync(IList<SpeechResultModel> speechResults)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.UpdateSpeechResultsAsync(ApplicationSettings.WebApiVersion, speechResults), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<IEnumerable<InformationMessage>>> GetInformationMessagesAsync(DateTime updatedAfter)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.GetInformationMessagesAsync(updatedAfter, ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<InformationMessage>> MarkMessageAsOpenedAsync(Guid informationMessageId)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.MarkMessageAsOpenedAsync(informationMessageId, ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> MarkMessagesAsOpenedAsync(IEnumerable<Guid> ids)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.MarkMessagesAsOpenedAsync(ApplicationSettings.WebApiVersion, ids), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task RefreshTokenIfNeededAsync()
        {
            var accessToken = _userSessionService.AccessToken;
            var daysToExpire = accessToken.ExpirationDate.Subtract(DateTimeOffset.UtcNow).TotalDays;
            if (daysToExpire > 30)
                return;

            var httpRequestResult = await WebServiceErrorHandler.HandleResponseAsync(() => MakeServiceCall(client => client.RefreshTokenAsync(ApplicationSettings.WebApiVersion), GetAuthHeaders())).ConfigureAwait(false);
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
