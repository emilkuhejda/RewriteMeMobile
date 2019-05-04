using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IFileItemRepository
    {
        Task<IEnumerable<FileItem>> GetAllAsync();

        Task<bool> AnyWaitingForSynchronizationAsync();

        Task InsertOrReplaceAsync(FileItem fileItem);

        Task InsertOrReplaceAllAsync(IEnumerable<FileItem> fileItems);

        Task UpdateRecognitionStateAsync(Guid fileItemId, RecognitionState recognitionState);

        Task<TimeSpan> GetProcessedFilesTotalTimeAsync();

        Task ClearAsync();
    }
}
