using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.DataAccess.Repositories
{
    public class FileItemRepository : IFileItemRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public FileItemRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<IEnumerable<FileItem>> GetAllAsync()
        {
            var entities = await _contextProvider.Context.FileItems.ToListAsync().ConfigureAwait(false);
            return entities.Select(x => x.ToFileItem());
        }

        public async Task InsertOrReplaceAsync(FileItem fileItem)
        {
            await _contextProvider.Context.InsertOrReplaceAsync(fileItem.ToFileItemEntity()).ConfigureAwait(false);
        }

        public async Task UpdateAllAsync(IEnumerable<FileItem> fileItems)
        {
            var fileItemEntities = fileItems.Select(x => x.ToFileItemEntity()).ToList();
            if (!fileItemEntities.Any())
                return;

            var existingEntities = await _contextProvider.Context.FileItems.ToListAsync().ConfigureAwait(false);
            var mergedFileItems = existingEntities.Where(x => fileItemEntities.All(e => e.Id != x.Id)).ToList();
            mergedFileItems.AddRange(fileItemEntities);

            await _contextProvider.Context.RunInTransactionAsync(database =>
            {
                database.DeleteAll<FileItemEntity>();
                database.InsertAll(mergedFileItems);
            }).ConfigureAwait(false);
        }
    }
}
