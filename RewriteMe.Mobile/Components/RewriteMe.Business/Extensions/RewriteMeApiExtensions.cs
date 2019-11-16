using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Extensions;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Extensions
{
    public static class RewriteMeApiExtensions
    {
        public static async Task<LastUpdates> GetLastUpdatesAsync(this IVoicipherAPI operations, string version, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetLastUpdatesWithHttpMessagesAsync(version, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<LastUpdates>(result);
            }
        }

        public static async Task<IEnumerable<FileItem>> GetFileItemsAsync(this IVoicipherAPI operations, string version, DateTime updatedAfter, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetFileItemsWithHttpMessagesAsync(version, updatedAfter, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<FileItem>>(result);
            }
        }

        public static async Task<IEnumerable<Guid>> GetDeletedFileItemIdsAsync(this IVoicipherAPI operations, string version, DateTime updatedAfter, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetDeletedFileItemIdsWithHttpMessagesAsync(version, updatedAfter, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<Guid?>>(result).Where(x => x.HasValue).Select(x => x.Value);
            }
        }

        public static async Task<TimeSpanWrapper> DeleteFileItemAsync(this IVoicipherAPI operations, string version, Guid fileItemId, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.DeleteFileItemWithHttpMessagesAsync(version, fileItemId, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<TimeSpanWrapper>(result);
            }
        }

        public static async Task<Ok> DeleteAllFileItemsAsync(this IVoicipherAPI operations, string version, IList<DeletedFileItem> fileItems, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            var deletedFileItemModels = fileItems.Select(x => x.ToDeletedFileItemModel()).ToList();
            using (var result = await operations.DeleteAllFileItemsWithHttpMessagesAsync(version, deletedFileItemModels, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<IEnumerable<TranscribeItem>> GetTranscribeItemsAllAsync(this IVoicipherAPI operations, string version, DateTime updatedAfter, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetTranscribeItemsAllWithHttpMessagesAsync(version, updatedAfter, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<TranscribeItem>>(result);
            }
        }

        public static async Task<TimeSpanWrapper> GetUserSubscriptionRemainingTimeAsync(this IVoicipherAPI operations, string version, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetSubscriptionRemainingTimeWithHttpMessagesAsync(version, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<TimeSpanWrapper>(result);
            }
        }

        public static async Task<TimeSpanWrapper> CreateUserSubscriptionAsync(this IVoicipherAPI operations, string version, BillingPurchase billingPurchase, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.CreateUserSubscriptionWithHttpMessagesAsync(version, billingPurchase, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<TimeSpanWrapper>(result);
            }
        }

        public static async Task<SpeechConfiguration> GetSpeechConfigurationAsync(this IVoicipherAPI operations, string version, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetSpeechConfigurationWithHttpMessagesAsync(version, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<SpeechConfiguration>(result);
            }
        }

        public static async Task<FileItem> UploadFileItemAsync(this IVoicipherAPI operations, string version, MediaFile mediaFile, DateTime dateCreated, Guid applicationId, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream(mediaFile.Source))
            using (var result = await operations.UploadFileItemWithHttpMessagesAsync(version, mediaFile.Name, mediaFile.Language, mediaFile.FileName, dateCreated, applicationId, stream, customHeaders, cancellationToken).ConfigureAwait(false))
            {
                return ParseBody<FileItem>(result);
            }
        }

        public static async Task<Ok> TranscribeFileItemAsync(this IVoicipherAPI operations, string version, Guid fileItemId, string language, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.TranscribeFileItemWithHttpMessagesAsync(version, fileItemId, language, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<byte[]> GetTranscribeAudioSourceAsync(this IVoicipherAPI operations, string version, Guid transcribeItemId, Dictionary<string, List<string>> customHeaders, CancellationToken cancellationToken)
        {
            using (var result = await operations.GetTranscribeAudioSourceWithHttpMessagesAsync(transcribeItemId, version, customHeaders, cancellationToken).ConfigureAwait(false))
            {
                return ParseBody<byte[]>(result);
            }
        }

        public static async Task<Ok> UpdateUserTranscriptAsync(this IVoicipherAPI operations, string version, Guid transcribeItemId, string transcript, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.UpdateUserTranscriptWithHttpMessagesAsync(version, transcribeItemId, applicationId, transcript, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<Ok> CreateSpeechResultAsync(this IVoicipherAPI operations, string version, Guid speechResultId, Guid recognizedAudioSampleId, string displayText, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.CreateSpeechResultWithHttpMessagesAsync(version, speechResultId, recognizedAudioSampleId, displayText, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<Ok> UpdateSpeechResultsAsync(this IVoicipherAPI operations, string version, IList<SpeechResultModel> speechResults, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.UpdateSpeechResultsWithHttpMessagesAsync(version, speechResults, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<IEnumerable<InformationMessage>> GetInformationMessagesAsync(this IVoicipherAPI operations, string version, DateTime updatedAfter, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetInformationMessagesWithHttpMessagesAsync(version, updatedAfter, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<InformationMessage>>(result);
            }
        }

        public static async Task<InformationMessage> MarkMessageAsOpenedAsync(this IVoicipherAPI operations, string version, Guid informationMessageId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.MarkMessageAsOpenedWithHttpMessagesAsync(version, informationMessageId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<InformationMessage>(result);
            }
        }

        public static async Task<Ok> MarkMessagesAsOpenedAsync(this IVoicipherAPI operations, string version, IEnumerable<Guid?> ids, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.MarkMessagesAsOpenedWithHttpMessagesAsync(version, ids.ToList(), customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<string> RefreshTokenAsync(this IVoicipherAPI operations, string version, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.RefreshTokenWithHttpMessagesAsync(version, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<string>(result);
            }
        }

        public static async Task<UserRegistration> RegisterUserAsync(this IVoicipherAPI operations, string version, RegistrationUserModel registrationUserModel, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.RegisterUserWithHttpMessagesAsync(version, registrationUserModel, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<UserRegistration>(result);
            }
        }

        public static async Task<Identity> UpdateUserAsync(this IVoicipherAPI operations, string version, UpdateUserModel updateUserModel, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.UpdateUserWithHttpMessagesAsync(version, updateUserModel, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Identity>(result);
            }
        }

        private static T ParseBody<T>(HttpOperationResponse<object> httpOperationResponse)
        {
            if (httpOperationResponse.Response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedCallException();

            if (httpOperationResponse.Response.StatusCode == HttpStatusCode.InternalServerError)
                throw new InternalServerErrorException();

            if (httpOperationResponse.Body is ProblemDetails problemDetails)
                throw new ProblemDetailsException(problemDetails);

            return (T)httpOperationResponse.Body;
        }
    }
}
