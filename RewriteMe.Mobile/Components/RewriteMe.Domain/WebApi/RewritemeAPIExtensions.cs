// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace RewriteMe.Domain.WebApi
{
    using Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for RewriteMeAPI.
    /// </summary>
    public static partial class RewriteMeAPIExtensions
    {
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='updatedAfter'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            public static IList<FileItem> GetFileItems(this IRewriteMeAPI operations, System.DateTime? updatedAfter = default(System.DateTime?), System.Guid? applicationId = default(System.Guid?))
            {
                return operations.GetFileItemsAsync(updatedAfter, applicationId).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='updatedAfter'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<FileItem>> GetFileItemsAsync(this IRewriteMeAPI operations, System.DateTime? updatedAfter = default(System.DateTime?), System.Guid? applicationId = default(System.Guid?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetFileItemsWithHttpMessagesAsync(updatedAfter, applicationId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='fileItemId'>
            /// </param>
            public static FileItem GetFileItem(this IRewriteMeAPI operations, System.Guid fileItemId)
            {
                return operations.GetFileItemAsync(fileItemId).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='fileItemId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<FileItem> GetFileItemAsync(this IRewriteMeAPI operations, System.Guid fileItemId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetFileItemWithHttpMessagesAsync(fileItemId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='name'>
            /// </param>
            /// <param name='language'>
            /// </param>
            /// <param name='fileName'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='file'>
            /// </param>
            public static object UploadFileItem(this IRewriteMeAPI operations, string name = default(string), string language = default(string), string fileName = default(string), System.Guid? applicationId = default(System.Guid?), Stream file = default(Stream))
            {
                return operations.UploadFileItemAsync(name, language, fileName, applicationId, file).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='name'>
            /// </param>
            /// <param name='language'>
            /// </param>
            /// <param name='fileName'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='file'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> UploadFileItemAsync(this IRewriteMeAPI operations, string name = default(string), string language = default(string), string fileName = default(string), System.Guid? applicationId = default(System.Guid?), Stream file = default(Stream), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UploadFileItemWithHttpMessagesAsync(name, language, fileName, applicationId, file, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='fileItemId'>
            /// </param>
            /// <param name='name'>
            /// </param>
            /// <param name='language'>
            /// </param>
            /// <param name='fileName'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='file'>
            /// </param>
            public static object UpdateFileItem(this IRewriteMeAPI operations, System.Guid? fileItemId = default(System.Guid?), string name = default(string), string language = default(string), string fileName = default(string), System.Guid? applicationId = default(System.Guid?), Stream file = default(Stream))
            {
                return operations.UpdateFileItemAsync(fileItemId, name, language, fileName, applicationId, file).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='fileItemId'>
            /// </param>
            /// <param name='name'>
            /// </param>
            /// <param name='language'>
            /// </param>
            /// <param name='fileName'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='file'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> UpdateFileItemAsync(this IRewriteMeAPI operations, System.Guid? fileItemId = default(System.Guid?), string name = default(string), string language = default(string), string fileName = default(string), System.Guid? applicationId = default(System.Guid?), Stream file = default(Stream), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UpdateFileItemWithHttpMessagesAsync(fileItemId, name, language, fileName, applicationId, file, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            public static Ok RemoveFileItem(this IRewriteMeAPI operations, string id)
            {
                return operations.RemoveFileItemAsync(id).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='id'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Ok> RemoveFileItemAsync(this IRewriteMeAPI operations, string id, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.RemoveFileItemWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='fileItemId'>
            /// </param>
            /// <param name='language'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            public static object TranscribeFileItem(this IRewriteMeAPI operations, System.Guid? fileItemId = default(System.Guid?), string language = default(string), System.Guid? applicationId = default(System.Guid?))
            {
                return operations.TranscribeFileItemAsync(fileItemId, language, applicationId).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='fileItemId'>
            /// </param>
            /// <param name='language'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> TranscribeFileItemAsync(this IRewriteMeAPI operations, System.Guid? fileItemId = default(System.Guid?), string language = default(string), System.Guid? applicationId = default(System.Guid?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.TranscribeFileItemWithHttpMessagesAsync(fileItemId, language, applicationId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static LastUpdates GetLastUpdates(this IRewriteMeAPI operations)
            {
                return operations.GetLastUpdatesAsync().GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<LastUpdates> GetLastUpdatesAsync(this IRewriteMeAPI operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetLastUpdatesWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='updatedAfter'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            public static IList<TranscribeItem> GetTranscribeItemsAll(this IRewriteMeAPI operations, System.DateTime? updatedAfter = default(System.DateTime?), System.Guid? applicationId = default(System.Guid?))
            {
                return operations.GetTranscribeItemsAllAsync(updatedAfter, applicationId).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='updatedAfter'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<TranscribeItem>> GetTranscribeItemsAllAsync(this IRewriteMeAPI operations, System.DateTime? updatedAfter = default(System.DateTime?), System.Guid? applicationId = default(System.Guid?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetTranscribeItemsAllWithHttpMessagesAsync(updatedAfter, applicationId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='transcribeItemId'>
            /// </param>
            public static byte[] GetTranscribeAudioSource(this IRewriteMeAPI operations, System.Guid transcribeItemId)
            {
                return operations.GetTranscribeAudioSourceAsync(transcribeItemId).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='transcribeItemId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<byte[]> GetTranscribeAudioSourceAsync(this IRewriteMeAPI operations, System.Guid transcribeItemId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetTranscribeAudioSourceWithHttpMessagesAsync(transcribeItemId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='transcribeItemId'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='transcript'>
            /// </param>
            public static Ok UpdateUserTranscript(this IRewriteMeAPI operations, System.Guid? transcribeItemId = default(System.Guid?), System.Guid? applicationId = default(System.Guid?), string transcript = default(string))
            {
                return operations.UpdateUserTranscriptAsync(transcribeItemId, applicationId, transcript).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='transcribeItemId'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='transcript'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Ok> UpdateUserTranscriptAsync(this IRewriteMeAPI operations, System.Guid? transcribeItemId = default(System.Guid?), System.Guid? applicationId = default(System.Guid?), string transcript = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UpdateUserTranscriptWithHttpMessagesAsync(transcribeItemId, applicationId, transcript, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='registerUserModel'>
            /// </param>
            public static UserSubscription RegisterUser(this IRewriteMeAPI operations, RegisterUserModel registerUserModel = default(RegisterUserModel))
            {
                return operations.RegisterUserAsync(registerUserModel).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='registerUserModel'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<UserSubscription> RegisterUserAsync(this IRewriteMeAPI operations, RegisterUserModel registerUserModel = default(RegisterUserModel), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.RegisterUserWithHttpMessagesAsync(registerUserModel, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='updatedAfter'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            public static IList<UserSubscription> GetUserSubscriptions(this IRewriteMeAPI operations, System.DateTime? updatedAfter = default(System.DateTime?), System.Guid? applicationId = default(System.Guid?))
            {
                return operations.GetUserSubscriptionsAsync(updatedAfter, applicationId).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='updatedAfter'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<UserSubscription>> GetUserSubscriptionsAsync(this IRewriteMeAPI operations, System.DateTime? updatedAfter = default(System.DateTime?), System.Guid? applicationId = default(System.Guid?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetUserSubscriptionsWithHttpMessagesAsync(updatedAfter, applicationId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='billingPurchase'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            public static object CreateUserSubscription(this IRewriteMeAPI operations, BillingPurchase billingPurchase = default(BillingPurchase), System.Guid? applicationId = default(System.Guid?))
            {
                return operations.CreateUserSubscriptionAsync(billingPurchase, applicationId).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='billingPurchase'>
            /// </param>
            /// <param name='applicationId'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> CreateUserSubscriptionAsync(this IRewriteMeAPI operations, BillingPurchase billingPurchase = default(BillingPurchase), System.Guid? applicationId = default(System.Guid?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateUserSubscriptionWithHttpMessagesAsync(billingPurchase, applicationId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
