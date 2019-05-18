using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.DataAccess.Repositories
{
    public class DeletedFileItemRepository : IDeletedFileItemRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public DeletedFileItemRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task InsertAsync(DeletedFileItem deletedFileItem)
        {
            await _contextProvider.Context.InsertAsync(deletedFileItem.ToDeletedFileItemEntity()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DeletedFileItem>> GetAllAsync()
        {
            var entities = await _contextProvider.Context.DeletedFileItems.ToListAsync().ConfigureAwait(false);

            return entities.Select(x => x.ToDeletedFileItem());
        }

        public async Task<TimeSpan> GetProcessedFilesTotalTimeAsync()
        {
            var deletedFileItem = await _contextProvider.Context.DeletedFileItems.Where(x => x.RecognitionState > RecognitionState.Prepared).ToListAsync().ConfigureAwait(false);
            var ticks = deletedFileItem.Select(x => x.TotalTime.Ticks).Sum();

            return TimeSpan.FromTicks(ticks);
        }

        public async Task ClearAsync()
        {
            await _contextProvider.Context.DeleteAllAsync<DeletedFileItemEntity>().ConfigureAwait(false);
        }
    }
}
