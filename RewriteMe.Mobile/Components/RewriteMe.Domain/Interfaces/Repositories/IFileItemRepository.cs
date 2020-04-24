using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IFileItemRepository
    {
        Task<FileItem> GetAsync(Guid fileItemId);

        Task<IEnumerable<FileItem>> GetAllAsync();

        Task InsertOrReplaceAsync(FileItem fileItem);

        Task InsertOrReplaceAllAsync(IEnumerable<FileItem> fileItems);

        Task DeleteAsync(IEnumerable<Guid> fileItemIds);

        Task DeleteAsync(Guid fileItemId);

        Task UpdateAsync(FileItem fileItem);

        Task UpdateRecognitionStateAsync(Guid fileItemId, RecognitionState recognitionState);

        Task UpdateUploadStatusAsync(Guid fileItemId, UploadStatus uploadStatus);

        Task SetUploadErrorCodeAsync(Guid fileItemId, ErrorCode? errorCode);

        Task SetTranscribeErrorCodeAsync(Guid fileItemId, ErrorCode? errorCode);

        Task ResetUploadStatusesAsync();

        Task ClearAsync();
    }
}
