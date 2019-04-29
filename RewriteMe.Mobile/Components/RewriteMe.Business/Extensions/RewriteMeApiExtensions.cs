using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.WebApi;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Extensions
{
    public static class RewriteMeApiExtensions
    {
        public static async Task<LastUpdates> GetLastUpdates(this IRewriteMeAPI operations, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetLastUpdatesWithHttpMessagesAsync(customHeaders))
            {
                return ParseBody<LastUpdates>(result.Body);
            }
        }

        public static async Task<IEnumerable<FileItem>> GetFileItemsAsync(this IRewriteMeAPI operations, DateTime updatedAfter, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetFileItemsWithHttpMessagesAsync(updatedAfter, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<FileItem>>(result.Body);
            }
        }

        public static async Task<IEnumerable<TranscribeItem>> GetTranscribeItemsAllAsync(this IRewriteMeAPI operations, DateTime updatedAfter, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.GetTranscribeItemsAllWithHttpMessagesAsync(updatedAfter, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<IEnumerable<TranscribeItem>>(result.Body);
            }
        }

        public static async Task<FileItem> CreateFileItemAsync(this IRewriteMeAPI operations, string name, string language, Dictionary<string, List<string>> customHeaders)
        {
            using (var result = await operations.CreateFileItemWithHttpMessagesAsync(name, language, null, customHeaders).ConfigureAwait(false))
            {
                return ParseBody<FileItem>(result.Body);
            }
        }

        public static async Task<Ok> RegisterUserAsync(this IRewriteMeAPI operations, RegisterUserModel registerUserModel)
        {
            using (var result = await operations.RegisterUserWithHttpMessagesAsync(registerUserModel).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result.Body);
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
