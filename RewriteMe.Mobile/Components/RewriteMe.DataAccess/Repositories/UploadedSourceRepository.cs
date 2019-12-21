using System;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Upload;

namespace RewriteMe.DataAccess.Repositories
{
    public class UploadedSourceRepository : IUploadedSourceRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public UploadedSourceRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<UploadedSource> GetFirstAsync()
        {
            var entity = await _contextProvider.Context.UploadedSources
                .OrderBy(x => x.DateCreated)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return entity?.ToUploadedSource();
        }

        public async Task AddAsync(UploadedSource uploadedSource)
        {
            await _contextProvider.Context.InsertAsync(uploadedSource.ToUploadedSourceEntity()).ConfigureAwait(false);
        }

        public async Task DeleteAsync(Guid uploadedSourceId)
        {
            await _contextProvider.Context.DeleteAsync<UploadedSourceEntity>(uploadedSourceId).ConfigureAwait(false);
        }

        public async Task ClearAsync()
        {
            await _contextProvider.Context.DeleteAllAsync<UploadedSourceEntity>().ConfigureAwait(false);
        }
    }
}
