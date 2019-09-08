using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRewriteMeWebService
    {
        Task<bool> IsAliveAsync();

        Task<HttpRequestResult<LastUpdates>> GetLastUpdatesAsync();

        Task<HttpRequestResult<IEnumerable<FileItem>>> GetFileItemsAsync(DateTime updatedAfter);

        Task<HttpRequestResult<IEnumerable<Guid>>> GetDeletedFileItemIdsAsync(DateTime updatedAfter);

        Task<HttpRequestResult<TimeSpanWrapper>> GetDeletedFileItemsTotalTimeAsync();

        Task<HttpRequestResult<TimeSpanWrapper>> DeleteFileItemAsync(Guid fileItemId);

        Task<HttpRequestResult<Ok>> DeleteAllFileItemsAsync(IList<DeletedFileItem> fileItems);

        Task<HttpRequestResult<IEnumerable<TranscribeItem>>> GetTranscribeItemsAllAsync(DateTime updatedAfter);

        Task<HttpRequestResult<IEnumerable<UserSubscription>>> GetUserSubscriptionsAsync(DateTime updatedAfter);

        Task<HttpRequestResult<UserSubscription>> CreateUserSubscriptionAsync(BillingPurchase billingPurchase);

        Task<HttpRequestResult<SpeechConfiguration>> GetSpeechConfigurationAsync();

        Task<HttpRequestResult<FileItem>> UploadFileItemAsync(MediaFile mediaFile, CancellationToken cancellationToken);

        Task<HttpRequestResult<Ok>> TranscribeFileItemAsync(Guid fileItemId, string language);

        Task<HttpRequestResult<byte[]>> GetTranscribeAudioSourceAsync(Guid transcribeItemId, CancellationToken cancellationToken);

        Task<HttpRequestResult<Ok>> UpdateUserTranscriptAsync(Guid transcribeItemId, string transcript);

        Task<HttpRequestResult<Ok>> CreateSpeechResultAsync(Guid speechResultId, Guid recognizedAudioSampleId, string displayText);

        Task<HttpRequestResult<Ok>> UpdateSpeechResultsAsync(IList<SpeechResultModel> speechResults);

        Task<HttpRequestResult<IEnumerable<InformationMessage>>> GetInformationMessagesAsync(DateTime updatedAfter);

        Task<HttpRequestResult<InformationMessage>> MarkMessageAsOpenedAsync(Guid informationMessageId);

        Task<HttpRequestResult<Ok>> MarkMessagesAsOpenedAsync(IEnumerable<Guid?> ids);

        Task<HttpRequestResult<RecognizedTime>> GetRecognizedTimeAsync();

        Task RefreshTokenIfNeededAsync();
    }
}
