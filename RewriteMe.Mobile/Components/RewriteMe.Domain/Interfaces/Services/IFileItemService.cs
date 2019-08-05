using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IFileItemService
    {
        event EventHandler TranscriptionStarted;

        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task<bool> AnyWaitingForSynchronizationAsync();

        Task<IEnumerable<FileItem>> GetAllAsync();

        Task DeleteAsync(FileItem fileItem);

        Task<FileItem> UploadAsync(MediaFile mediaFile);

        Task<bool> CanTranscribeAsync();

        Task<bool> CanTranscribeAsync(TimeSpan fileTime);

        Task TranscribeAsync(Guid fileItemId, string language);
    }
}
