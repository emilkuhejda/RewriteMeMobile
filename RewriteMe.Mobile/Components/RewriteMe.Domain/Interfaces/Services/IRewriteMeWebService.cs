﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRewriteMeWebService
    {
        Task<HttpRequestResult<LastUpdates>> GetLastUpdates();

        Task<HttpRequestResult<IEnumerable<FileItem>>> GetFileItemsAsync(DateTime updatedAfter);

        Task<HttpRequestResult<IEnumerable<TranscribeItem>>> GetTranscribeItemsAllAsync(DateTime updatedAfter);

        Task<HttpRequestResult<IEnumerable<UserSubscription>>> GetUserSubscriptionsAsync(DateTime updatedAfter);

        Task<HttpRequestResult<FileItem>> UploadFileItemAsync(MediaFile mediaFile);

        Task<HttpRequestResult<Ok>> TranscribeFileItemAsync(Guid fileItemId, string language);

        Task<HttpRequestResult<Ok>> UpdateUserTranscriptAsync(Guid transcribeItemId, string transcript);
    }
}
