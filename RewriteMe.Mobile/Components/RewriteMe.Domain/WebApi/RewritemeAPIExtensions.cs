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
            public static IList<FileItem> GetFileItems(this IRewriteMeAPI operations, System.DateTime? updatedAfter = default(System.DateTime?))
            {
                return operations.GetFileItemsAsync(updatedAfter).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='updatedAfter'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<FileItem>> GetFileItemsAsync(this IRewriteMeAPI operations, System.DateTime? updatedAfter = default(System.DateTime?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetFileItemsWithHttpMessagesAsync(updatedAfter, null, cancellationToken).ConfigureAwait(false))
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
            public static object CreateFileItem(this IRewriteMeAPI operations, string name = default(string), string language = default(string))
            {
                return operations.CreateFileItemAsync(name, language).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='name'>
            /// </param>
            /// <param name='language'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> CreateFileItemAsync(this IRewriteMeAPI operations, string name = default(string), string language = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.CreateFileItemWithHttpMessagesAsync(name, language, null, cancellationToken).ConfigureAwait(false))
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
            public static object UpdateFileItem(this IRewriteMeAPI operations, System.Guid? fileItemId = default(System.Guid?), string name = default(string), string language = default(string))
            {
                return operations.UpdateFileItemAsync(fileItemId, name, language).GetAwaiter().GetResult();
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
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> UpdateFileItemAsync(this IRewriteMeAPI operations, System.Guid? fileItemId = default(System.Guid?), string name = default(string), string language = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UpdateFileItemWithHttpMessagesAsync(fileItemId, name, language, null, cancellationToken).ConfigureAwait(false))
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
            /// <param name='transcribeFileItemModel'>
            /// </param>
            public static object TranscribeFileItem(this IRewriteMeAPI operations, TranscribeFileItemModel transcribeFileItemModel = default(TranscribeFileItemModel))
            {
                return operations.TranscribeFileItemAsync(transcribeFileItemModel).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='transcribeFileItemModel'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> TranscribeFileItemAsync(this IRewriteMeAPI operations, TranscribeFileItemModel transcribeFileItemModel = default(TranscribeFileItemModel), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.TranscribeFileItemWithHttpMessagesAsync(transcribeFileItemModel, null, cancellationToken).ConfigureAwait(false))
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
            /// <param name='fileItemId'>
            /// </param>
            /// <param name='updatedAfter'>
            /// </param>
            public static IList<TranscribeItem> GetTranscribeItems(this IRewriteMeAPI operations, System.Guid fileItemId, System.DateTime? updatedAfter = default(System.DateTime?))
            {
                return operations.GetTranscribeItemsAsync(fileItemId, updatedAfter).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='fileItemId'>
            /// </param>
            /// <param name='updatedAfter'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<IList<TranscribeItem>> GetTranscribeItemsAsync(this IRewriteMeAPI operations, System.Guid fileItemId, System.DateTime? updatedAfter = default(System.DateTime?), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetTranscribeItemsWithHttpMessagesAsync(fileItemId, updatedAfter, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='transcribeItemId'>
            /// </param>
            public static FileContentResult GetTranscribeAudioSource(this IRewriteMeAPI operations, System.Guid transcribeItemId)
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
            public static async Task<FileContentResult> GetTranscribeAudioSourceAsync(this IRewriteMeAPI operations, System.Guid transcribeItemId, CancellationToken cancellationToken = default(CancellationToken))
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
            /// <param name='transcript'>
            /// </param>
            public static Ok UpdateUserTranscript(this IRewriteMeAPI operations, System.Guid? transcribeItemId = default(System.Guid?), string transcript = default(string))
            {
                return operations.UpdateUserTranscriptAsync(transcribeItemId, transcript).GetAwaiter().GetResult();
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='transcribeItemId'>
            /// </param>
            /// <param name='transcript'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Ok> UpdateUserTranscriptAsync(this IRewriteMeAPI operations, System.Guid? transcribeItemId = default(System.Guid?), string transcript = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.UpdateUserTranscriptWithHttpMessagesAsync(transcribeItemId, transcript, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='registerUserModel'>
            /// </param>
            public static Ok RegisterUser(this IRewriteMeAPI operations, RegisterUserModel registerUserModel = default(RegisterUserModel))
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
            public static async Task<Ok> RegisterUserAsync(this IRewriteMeAPI operations, RegisterUserModel registerUserModel = default(RegisterUserModel), CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.RegisterUserWithHttpMessagesAsync(registerUserModel, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
