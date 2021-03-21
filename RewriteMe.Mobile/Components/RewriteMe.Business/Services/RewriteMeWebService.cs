using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Enums;
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
        private const int TimeoutSeconds = 5;

        private readonly IInternalValueService _internalValueService;
        private readonly IUserSessionService _userSessionService;
        private readonly ILogger _logger;

        public RewriteMeWebService(
            IInternalValueService internalValueService,
            IUserSessionService userSessionService,
            ILoggerFactory loggerFactory,
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
            : base(webServiceErrorHandler, applicationSettings)
        {
            _internalValueService = internalValueService;
            _userSessionService = userSessionService;
            _logger = loggerFactory.CreateLogger(typeof(RewriteMeWebService));
        }

        public async Task<bool> IsAliveAsync()
        {
            try
            {
                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds)))
                {
                    try
                    {
                        var rewriteMeClient = new RewriteMeClient(ApplicationSettings.WebApiUrl, Client);
                        var isAlive = await rewriteMeClient.IsAliveAsync(ApplicationSettings.WebApiVersion, cancellationTokenSource.Token).ConfigureAwait(false);
                        if (!isAlive)
                        {
                            return await UpdateWebApiToDefaultAsync().ConfigureAwait(false);
                        }

                        return true;
                    }
                    catch (Exception)
                    {
                        return await UpdateWebApiToDefaultAsync().ConfigureAwait(false);
                    }
                }
            }
            catch (Exception exception)
            {
                var message = "Exception during 'is-alive' web service request.";
                _logger.Warning($"{message} {exception}");

                return false;
            }
        }

        private async Task<bool> UpdateWebApiToDefaultAsync()
        {
            try
            {
                if (ApplicationSettings.WebApiUrl.Equals(InternalValues.ApiUrl.DefaultValue, StringComparison.OrdinalIgnoreCase))
                    return false;

                await _internalValueService.UpdateValueAsync(InternalValues.ApiUrl, InternalValues.ApiUrl.DefaultValue).ConfigureAwait(false);
                await ApplicationSettings.InitializeAsync().ConfigureAwait(false);

                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds)))
                {
                    var rewriteMeClient = new RewriteMeClient(ApplicationSettings.WebApiUrl, Client);
                    return await rewriteMeClient.IsAliveAsync(ApplicationSettings.WebApiVersion, cancellationTokenSource.Token).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
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
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.GetSubscriptionRemainingTimeAsync(ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<TimeSpanWrapper>> CreateUserSubscriptionAsync(CreateUserSubscriptionInputModel createUserSubscriptionInputModel)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.CreateUserSubscriptionAsync(ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion, createUserSubscriptionInputModel), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<SpeechConfiguration>> GetSpeechConfigurationAsync()
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.GetSpeechConfigurationAsync(ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<FileItem>> CreateFileItemAsync(MediaFile mediaFile, CancellationToken cancellationToken)
        {
            var transcriptionStartTime = mediaFile.IsTimeFrame ? mediaFile.TranscriptionStartTime : TimeSpan.Zero;
            var transcriptionEndTime = mediaFile.IsTimeFrame ? mediaFile.TranscriptionEndTime : TimeSpan.Zero;

            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.CreateFileItemAsync(
                    mediaFile.Name,
                    mediaFile.Language,
                    mediaFile.FileName,
                    mediaFile.IsPhoneCall,
                    (int)transcriptionStartTime.TotalSeconds,
                    (int)transcriptionEndTime.TotalSeconds,
                    DateTime.Now,
                    ApplicationSettings.ApplicationId,
                    ApplicationSettings.WebApiVersion,
                    cancellationToken), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> UploadChunkFileAsync(Guid fileItemId, int order, byte[] source, CancellationToken cancellationToken)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.UploadChunkFileAsync(fileItemId, order, ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion, source, cancellationToken), GetAuthHeaders())
            ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<FileItem>> SubmitChunksAsync(Guid fileItemId, int chunksCount, CancellationToken cancellationToken)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.SubmitChunksAsync(fileItemId, chunksCount, ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion, cancellationToken), GetAuthHeaders())
            ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> DeleteChunksAsync(Guid fileItemId)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.DeleteChunksAsync(fileItemId, ApplicationSettings.ApplicationId, ApplicationSettings.WebApiVersion), GetAuthHeaders())
            ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> TranscribeFileItemAsync(Guid fileItemId, string language, bool isPhoneCall, int transcriptionStartTimeSeconds, int transcriptionEndTimeSeconds)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.TranscribeFileItemAsync(
                    fileItemId,
                    language,
                    isPhoneCall,
                    transcriptionStartTimeSeconds,
                    transcriptionEndTimeSeconds,
                    ApplicationSettings.ApplicationId,
                    ApplicationSettings.WebApiVersion), GetAuthHeaders())
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
            var model = new UpdateUserTranscriptInputModel
            {
                ApplicationId = ApplicationSettings.ApplicationId,
                TranscribeItemId = transcribeItemId,
                Transcript = transcript
            };

            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.UpdateUserTranscriptAsync(ApplicationSettings.WebApiVersion, model), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> CreateSpeechResultAsync(Guid speechResultId, Guid recognizedAudioSampleId, string displayText)
        {
            var model = new CreateSpeechResultInputModel
            {
                SpeechResultId = speechResultId,
                RecognizedAudioSampleId = recognizedAudioSampleId,
                DisplayText = displayText
            };

            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.CreateSpeechResultAsync(ApplicationSettings.WebApiVersion, model), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<TimeSpanWrapper>> UpdateSpeechResultsAsync(IList<SpeechResultInputModel> speechResults)
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

        public async Task<HttpRequestResult<Ok>> UpdateLanguageAsync(Language language)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.UpdateLanguageAsync(ApplicationSettings.ApplicationId, (int)language, ApplicationSettings.WebApiVersion), GetAuthHeaders())
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Ok>> DeleteUserAsync()
        {
            var userSession = await _userSessionService.GetUserSessionAsync().ConfigureAwait(false);
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.DeleteUserAsync(userSession.Email, ApplicationSettings.WebApiVersion), GetAuthHeaders())
            ).ConfigureAwait(false);
        }

        public async Task<bool> RefreshTokenIfNeededAsync()
        {
            var accessToken = _userSessionService.AccessToken;
            var daysToExpire = accessToken.ExpirationDate.Subtract(DateTimeOffset.UtcNow).TotalDays;
            if (daysToExpire > 30)
                return true;

            var refreshToken = _userSessionService.GetRefreshToken();
            var httpRequestResult = await WebServiceErrorHandler.HandleResponseAsync(() => MakeServiceCall(client => client.RefreshTokenAsync(ApplicationSettings.WebApiVersion), GetAuthHeaders(refreshToken))).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                _userSessionService.SetToken(httpRequestResult.Payload);

                return true;
            }

            return false;
        }

        private CustomHeadersDictionary GetAuthHeaders(string accessToken)
        {
            return new CustomHeadersDictionary().AddBearerToken(accessToken);
        }

        private CustomHeadersDictionary GetAuthHeaders()
        {
            var accessToken = _userSessionService.GetToken();
            return new CustomHeadersDictionary().AddBearerToken(accessToken);
        }
    }
}
