using System.IO;
using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Required;
using SQLite;

namespace RewriteMe.DataAccess.Providers
{
    public class AppDbContextProvider : IAppDbContextProvider
    {
        private const string DatabaseName = "RewriteMe.db3";

        private readonly IDirectoryProvider _directoryProvider;
        private readonly object _lockObject = new object();

        private IAppDbContext _dbContext;

        public AppDbContextProvider(IDirectoryProvider directoryProvider)
        {
            _directoryProvider = directoryProvider;
        }

        public IAppDbContext Context
        {
            get
            {
                lock (_lockObject)
                {
                    if (_dbContext != null)
                        return _dbContext;

                    return _dbContext = Create();
                }
            }
        }

        private IAppDbContext Create()
        {
            var dbDirectory = _directoryProvider.GetPath();
            var dbPath = Path.Combine(dbDirectory, DatabaseName);
            var connection = new SQLiteAsyncConnection(dbPath);

            return _dbContext = new AppDbContext(connection);
        }

        public async Task CloseAsync()
        {
            if (_dbContext == null)
                return;

            await _dbContext.CloseAsync().ConfigureAwait(false);
            _dbContext = null;
        }
    }
}
