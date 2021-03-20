using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRewriteMeWebService
    {
        Task<bool> IsAliveAsync();

        Task<HttpRequestResult<LastUpdates>> GetLastUpdatesAsync();

        Task<HttpRequestResult<IEnumerable<FileItem>>> GetFileItemsAsync(DateTime updatedAfter);

        Task<HttpRequestResult<IEnumerable<Guid>>> GetDeletedFileItemIdsAsync(DateTime updatedAfter);

        Task<HttpRequestResult<Ok>> DeleteFileItemAsync(Guid fileItemId);

        Task<HttpRequestResult<Ok>> DeleteAllFileItemsAsync(IList<DeletedFileItem> fileItems);

        Task<HttpRequestResult<IEnumerable<TranscribeItem>>> GetTranscribeItemsAllAsync(DateTime updatedAfter);

        Task<HttpRequestResult<TimeSpanWrapper>> GetUserSubscriptionRemainingTimeAsync();

        Task<HttpRequestResult<TimeSpanWrapper>> CreateUserSubscriptionAsync(CreateUserSubscriptionInputModel createUserSubscriptionInputModel);

        Task<HttpRequestResult<SpeechConfiguration>> GetSpeechConfigurationAsync();

        Task<HttpRequestResult<FileItem>> CreateFileItemAsync(MediaFile mediaFile, CancellationToken cancellationToken);

        Task<HttpRequestResult<Ok>> UploadChunkFileAsync(Guid fileItemId, int order, byte[] source, CancellationToken cancellationToken);

        Task<HttpRequestResult<FileItem>> SubmitChunksAsync(Guid fileItemId, int chunksCount, CancellationToken cancellationToken);

        Task<HttpRequestResult<Ok>> DeleteChunksAsync(Guid fileItemId);

        Task<HttpRequestResult<Ok>> TranscribeFileItemAsync(Guid fileItemId, string language, bool isPhoneCall, int transcriptionStartTimeSeconds, int transcriptionEndTimeSeconds);

        Task<HttpRequestResult<byte[]>> GetTranscribeAudioSourceAsync(Guid transcribeItemId, CancellationToken cancellationToken);

        Task<HttpRequestResult<Ok>> UpdateUserTranscriptAsync(Guid transcribeItemId, string transcript);

        Task<HttpRequestResult<Ok>> CreateSpeechResultAsync(Guid speechResultId, Guid recognizedAudioSampleId, string displayText);

        Task<HttpRequestResult<TimeSpanWrapper>> UpdateSpeechResultsAsync(IList<SpeechResultInputModel> speechResults);

        Task<HttpRequestResult<IEnumerable<InformationMessage>>> GetInformationMessagesAsync(DateTime updatedAfter);

        Task<HttpRequestResult<InformationMessage>> MarkMessageAsOpenedAsync(Guid informationMessageId);

        Task<HttpRequestResult<Ok>> MarkMessagesAsOpenedAsync(IEnumerable<Guid> ids);

        Task<HttpRequestResult<Ok>> UpdateLanguageAsync(Language language);

        Task<HttpRequestResult<Ok>> DeleteUserAsync();

        Task<bool> RefreshTokenIfNeededAsync();
    }
}
