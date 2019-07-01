using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IRecordedItemRepository
    {
        Task InsertAsync(RecordedItem recordedItem);

        Task<RecordedItem> GetAsync(Guid recordedItemId);
    }
}
