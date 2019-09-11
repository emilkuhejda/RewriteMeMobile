using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface ITranscribeItemRepository
    {
        Task<IEnumerable<TranscribeItem>> GetAllForAudioSourceSynchronizationAsync();

        Task<IEnumerable<TranscribeItem>> GetAllAsync(Guid fileItemId);

        Task InsertOrReplaceAllAsync(IEnumerable<TranscribeItem> transcribeItems);

        Task UpdateAsync(TranscribeItem transcribeItem);

        Task UpdateAllAsync(IEnumerable<TranscribeItem> transcribeItems);

        Task<IEnumerable<TranscribeItem>> GetPendingAsync();
    }
}
