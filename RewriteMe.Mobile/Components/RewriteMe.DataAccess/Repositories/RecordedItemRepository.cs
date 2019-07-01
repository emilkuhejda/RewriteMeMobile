using System;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.DataAccess.Repositories
{
    public class RecordedItemRepository : IRecordedItemRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public RecordedItemRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task InsertAsync(RecordedItem recordedItem)
        {
            await _contextProvider.Context.InsertAsync(recordedItem.ToRecordedItemEntity()).ConfigureAwait(false);
        }

        public async Task<RecordedItem> GetAsync(Guid recordedItemId)
        {
            var entity = await _contextProvider.Context.GetWithChildrenAsync<RecordedItemEntity>(recordedItemId).ConfigureAwait(false);
            return entity.ToRecordedItem();
        }
    }
}
