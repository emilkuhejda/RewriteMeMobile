using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IFileItemService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task<IEnumerable<FileItem>> GetAllAsync();

        Task<FileItem> UploadAsync(MediaFile mediaFile);

        Task<bool> TranscribeAsync(Guid fileItemId, string language);
    }
}
