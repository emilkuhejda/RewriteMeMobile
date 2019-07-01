using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;

namespace RewriteMe.DataAccess.Repositories
{
    public class RecordedItemRepository : IRecordedItemRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public RecordedItemRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }
    }
}
