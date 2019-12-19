using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IFileItemService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task<bool> AnyWaitingForSynchronizationAsync();

        Task<IEnumerable<FileItem>> GetAllAsync();

        Task DeleteAsync(FileItem fileItem);

        Task<FileItem> UploadAsync(MediaFile mediaFile, CancellationToken cancellationToken);

        Task<bool> CanTranscribeAsync();

        Task TranscribeAsync(Guid fileItemId, string language);
    }
}
