using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IFileItemService
    {
        event EventHandler<UploadProgressEventArgs> UploadProgress;

        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task<bool> AnyWaitingForSynchronizationAsync();

        Task<FileItem> GetAsync(Guid fileItemId);

        Task<IEnumerable<FileItem>> GetAllAsync();

        Task DeleteAsync(FileItem fileItem);

        Task<FileItem> CreateAsync(MediaFile mediaFile, CancellationToken cancellationToken);

        Task<bool> UploadSourceFileAsync(Guid fileItemId, byte[] source, CancellationToken cancellationToken);

        Task DeleteChunksAsync(Guid fileItemId);

        Task<bool> CanTranscribeAsync();

        Task TranscribeAsync(Guid fileItemId, string language);

        Task UpdateUploadStatusAsync(Guid fileItemId, UploadStatus uploadStatus);

        Task SetUploadErrorCodeAsync(Guid fileItemId, ErrorCode? errorCode);

        Task SetTranscribeErrorCodeAsync(Guid fileItemId, ErrorCode? errorCode);
    }
}
