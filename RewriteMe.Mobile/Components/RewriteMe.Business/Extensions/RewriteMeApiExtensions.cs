﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public static async Task<LastUpdates> GetLastUpdatesAsync(this IRewriteMeAPI operations, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetLastUpdatesWithHttpMessagesAsync(customHeaders).ConfigureAwait(false))
            {
                return ParseBody<LastUpdates>(result);
            }
        }

        public static async Task<IEnumerable<FileItem>> GetFileItemsAsync(this IRewriteMeAPI operations, DateTime updatedAfter, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetFileItemsWithHttpMessagesAsync(updatedAfter, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<FileItem>>(result);
            }
        }

        public static async Task<IEnumerable<Guid>> GetDeletedFileItemIdsAsync(this IRewriteMeAPI operations, DateTime updatedAfter, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetDeletedFileItemIdsWithHttpMessagesAsync(updatedAfter, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<Guid?>>(result).Where(x => x.HasValue).Select(x => x.Value);
            }
        }

        public static async Task<TimeSpanWrapper> GetDeletedFileItemsTotalTimeAsync(this IRewriteMeAPI operations, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetDeletedFileItemsTotalTimeWithHttpMessagesAsync(customHeaders).ConfigureAwait(false))
            {
                return ParseBody<TimeSpanWrapper>(result);
            }
        }

        public static async Task<TimeSpanWrapper> DeleteFileItemAsync(this IRewriteMeAPI operations, Guid fileItemId, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.DeleteFileItemWithHttpMessagesAsync(fileItemId, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<TimeSpanWrapper>(result);
            }
        }

        public static async Task<Ok> DeleteAllFileItemsAsync(this IRewriteMeAPI operations, IList<DeletedFileItem> fileItems, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            var deletedFileItemModels = fileItems.Select(x => x.ToDeletedFileItemModel()).ToList();
            using (var result = await operations.DeleteAllFileItemsWithHttpMessagesAsync(deletedFileItemModels, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<IEnumerable<TranscribeItem>> GetTranscribeItemsAllAsync(this IRewriteMeAPI operations, DateTime updatedAfter, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetTranscribeItemsAllWithHttpMessagesAsync(updatedAfter, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<TranscribeItem>>(result);
            }
        }

        public static async Task<IEnumerable<UserSubscription>> GetUserSubscriptionsAsync(this IRewriteMeAPI operations, DateTime updatedAfter, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetUserSubscriptionsWithHttpMessagesAsync(updatedAfter, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<UserSubscription>>(result);
            }
        }

        public static async Task<UserSubscription> CreateUserSubscriptionAsync(this IRewriteMeAPI operations, BillingPurchase billingPurchase, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.CreateUserSubscriptionWithHttpMessagesAsync(billingPurchase, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<UserSubscription>(result);
            }
        }

        public static async Task<SpeechConfiguration> GetSpeechConfigurationAsync(this IRewriteMeAPI operations, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetSpeechConfigurationWithHttpMessagesAsync(customHeaders).ConfigureAwait(false))
            {
                return ParseBody<SpeechConfiguration>(result);
            }
        }

        public static async Task<FileItem> UploadFileItemAsync(this IRewriteMeAPI operations, MediaFile mediaFile, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.UploadFileItemWithHttpMessagesAsync(mediaFile.Name, mediaFile.Language, mediaFile.FileName, applicationId, mediaFile.Stream, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<FileItem>(result);
            }
        }

        public static async Task<Ok> TranscribeFileItemAsync(this IRewriteMeAPI operations, Guid fileItemId, string language, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.TranscribeFileItemWithHttpMessagesAsync(fileItemId, language, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<byte[]> GetTranscribeAudioSourceAsync(this IRewriteMeAPI operations, Guid transcribeItemId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetTranscribeAudioSourceWithHttpMessagesAsync(transcribeItemId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<byte[]>(result);
            }
        }

        public static async Task<Ok> UpdateUserTranscriptAsync(this IRewriteMeAPI operations, Guid transcribeItemId, string transcript, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.UpdateUserTranscriptWithHttpMessagesAsync(transcribeItemId, applicationId, transcript, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<Ok> CreateSpeechResultAsync(this IRewriteMeAPI operations, Guid speechResultId, Guid recognizedAudioSampleId, string displayText, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.CreateSpeechResultWithHttpMessagesAsync(speechResultId, recognizedAudioSampleId, displayText, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<Ok> UpdateSpeechResultsAsync(this IRewriteMeAPI operations, IList<SpeechResultModel> speechResults, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.UpdateSpeechResultsWithHttpMessagesAsync(speechResults, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result);
            }
        }

        public static async Task<RecognizedTime> GetRecognizedTimeAsync(this IRewriteMeAPI operations, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetRecognizedTimeWithHttpMessagesAsync(customHeaders).ConfigureAwait(false))
            {
                return ParseBody<RecognizedTime>(result);
            }
        }

        public static async Task<RegistrationModel> RegisterUserAsync(this IRewriteMeAPI operations, RegisterUserModel registerUserModel, Dictionary<string, List<string>> customHeaders)
        {
            using (HttpOperationResponse<object> result = await operations.RegisterUserWithHttpMessagesAsync(registerUserModel, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<RegistrationModel>(result);
            }
        }

        private static T ParseBody<T>(HttpOperationResponse<object> httpOperationResponse)
        {
            if (httpOperationResponse.Response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedCallException();

            if (httpOperationResponse.Body is ProblemDetails problemDetails)
                throw new ProblemDetailsException(problemDetails);

            return (T)httpOperationResponse.Body;
        }
    }
}
