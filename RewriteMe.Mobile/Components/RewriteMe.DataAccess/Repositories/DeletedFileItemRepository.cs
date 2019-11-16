using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Repositories;

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

        public async Task ClearAsync()
        {
            await _contextProvider.Context.DeleteAllAsync<DeletedFileItemEntity>().ConfigureAwait(false);
        }
    }
}
