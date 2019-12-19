using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.DataAccess.Repositories
{
    public class TranscribeItemRepository : ITranscribeItemRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public TranscribeItemRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<IEnumerable<TranscribeItem>> GetAllForAudioSourceSynchronizationAsync()
        {
            var entities = await _contextProvider.Context
                .GetAllWithChildrenAsync<TranscribeItemEntity>(x => true)
                .ConfigureAwait(false);

            return entities.Where(x => x.TranscriptAudioSource == null).Select(x => x.ToTranscribeItem());
        }

        public async Task<IEnumerable<TranscribeItem>> GetAllAsync(Guid fileItemId)
        {
            var entities = await _contextProvider.Context.TranscribeItems
                .Where(x => x.FileItemId == fileItemId)
                .ToListAsync()
                .ConfigureAwait(false);

            return entities.Select(x => x.ToTranscribeItem());
        }

        public async Task InsertOrReplaceAllAsync(IEnumerable<TranscribeItem> transcribeItems)
        {
            var transcribeItemsEntities = transcribeItems.Select(x => x.ToTranscribeItemEntity()).ToList();
            if (!transcribeItemsEntities.Any())
                return;

            var existingEntities = await _contextProvider.Context.TranscribeItems.ToListAsync().ConfigureAwait(false);
            var mergedTranscribeItems = existingEntities.Where(x => transcribeItemsEntities.All(e => e.Id != x.Id)).ToList();
            mergedTranscribeItems.AddRange(transcribeItemsEntities);

            await _contextProvider.Context.RunInTransactionAsync(database =>
            {
                database.DeleteAll<TranscribeItemEntity>();
                database.InsertAll(mergedTranscribeItems);
            }).ConfigureAwait(false);
        }

        public async Task UpdateAsync(TranscribeItem transcribeItem)
        {
            await _contextProvider.Context.UpdateAsync(transcribeItem.ToTranscribeItemEntity()).ConfigureAwait(false);
        }

        public async Task UpdateAllAsync(IEnumerable<TranscribeItem> transcribeItems)
        {
            var entities = transcribeItems.Select(x => x.ToTranscribeItemEntity());
            await _contextProvider.Context.UpdateAllAsync(entities).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TranscribeItem>> GetPendingAsync()
        {
            var entities = await _contextProvider.Context.GetAllWithChildrenAsync<TranscribeItemEntity>(x => x.IsPendingSynchronization).ConfigureAwait(false);

            return entities.Select(x => x.ToTranscribeItem());
        }
    }
}
