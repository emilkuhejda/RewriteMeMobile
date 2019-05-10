using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Extensions
{
    public static class RewriteMeApiExtensions
    {
        public static async Task<LastUpdates> GetLastUpdates(this IRewriteMeAPI operations, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetLastUpdatesWithHttpMessagesAsync(customHeaders).ConfigureAwait(false))
            {
                return ParseBody<LastUpdates>(result.Body);
            }
        }

        public static async Task<IEnumerable<FileItem>> GetFileItemsAsync(this IRewriteMeAPI operations, DateTime updatedAfter, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetFileItemsWithHttpMessagesAsync(updatedAfter, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<FileItem>>(result.Body);
            }
        }

        public static async Task<IEnumerable<TranscribeItem>> GetTranscribeItemsAllAsync(this IRewriteMeAPI operations, DateTime updatedAfter, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetTranscribeItemsAllWithHttpMessagesAsync(updatedAfter, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<TranscribeItem>>(result.Body);
            }
        }

        public static async Task<IEnumerable<UserSubscription>> GetUserSubscriptionsAsync(this IRewriteMeAPI operations, DateTime updatedAfter, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetUserSubscriptionsWithHttpMessagesAsync(updatedAfter, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<UserSubscription>>(result.Body);
            }
        }

        public static async Task<IEnumerable<SubscriptionProduct>> GetSubscriptionProductsAsync(this IRewriteMeAPI operations, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetAvailableProductsWithHttpMessagesAsync(customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<SubscriptionProduct>>(result.Body);
            }
        }

        public static async Task<UserSubscription> CreateUserSubscriptionAsync(this IRewriteMeAPI operations, BillingPurchase billingPurchase, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.CreateUserSubscriptionWithHttpMessagesAsync(billingPurchase, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<UserSubscription>(result.Body);
            }
        }

        public static async Task<FileItem> UploadFileItemAsync(this IRewriteMeAPI operations, MediaFile mediaFile, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.UploadFileItemWithHttpMessagesAsync(mediaFile.Name, mediaFile.Language, mediaFile.FileName, applicationId, mediaFile.Stream, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<FileItem>(result.Body);
            }
        }

        public static async Task<Ok> TranscribeFileItemAsync(this IRewriteMeAPI operations, Guid fileItemId, string language, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.TranscribeFileItemWithHttpMessagesAsync(fileItemId, language, applicationId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result.Body);
            }
        }

        public static async Task<byte[]> GetTranscribeAudioSourceAsync(this IRewriteMeAPI operations, Guid transcribeItemId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetTranscribeAudioSourceWithHttpMessagesAsync(transcribeItemId, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<byte[]>(result.Body);
            }
        }

        public static async Task<Ok> UpdateUserTranscriptAsync(this IRewriteMeAPI operations, Guid transcribeItemId, string transcript, Guid applicationId, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.UpdateUserTranscriptWithHttpMessagesAsync(transcribeItemId, applicationId, transcript, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result.Body);
            }
        }

        public static async Task<UserSubscription> RegisterUserAsync(this IRewriteMeAPI operations, RegisterUserModel registerUserModel)
        {
            using (var result = await operations.RegisterUserWithHttpMessagesAsync(registerUserModel).ConfigureAwait(false))
            {
                return ParseBody<UserSubscription>(result.Body);
            }
        }

        private static T ParseBody<T>(object body)
        {
            if (body is ProblemDetails problemDetails)
                throw new ProblemDetailsException(problemDetails);

            return (T)body;
        }
    }
}
