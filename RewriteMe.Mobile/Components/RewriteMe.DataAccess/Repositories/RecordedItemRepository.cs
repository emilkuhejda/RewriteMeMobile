using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task DeleteAsync(Guid recordedItemId)
        {
            await _contextProvider.Context.DeleteWithChildrenAsync<RecordedItemEntity>(recordedItemId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RecordedItem>> GetAllAsync()
        {
            var entities = await _contextProvider.Context.GetAllWithChildrenAsync<RecordedItemEntity>(x => true).ConfigureAwait(false);
            return entities.Select(x => x.ToRecordedItem());
        }

        public async Task<RecordedItem> GetAsync(Guid recordedItemId)
        {
            var entity = await _contextProvider.Context.GetWithChildrenAsync<RecordedItemEntity>(recordedItemId).ConfigureAwait(false);
            return entity.ToRecordedItem();
        }

        public async Task UpdateAsync(RecordedItem recordedItem)
        {
            await _contextProvider.Context.UpdateAsync(recordedItem.ToRecordedItemEntity()).ConfigureAwait(false);
        }

        public async Task ClearAsync()
        {
            await _contextProvider.Context.DeleteAllAsync<RecordedItemEntity>().ConfigureAwait(false);
        }
    }
}
