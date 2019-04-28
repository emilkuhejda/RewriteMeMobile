using System;
using System.Threading.Tasks;
using RewriteMe.DataAccess.Entities;
using SQLite;

namespace RewriteMe.DataAccess
{
    public class AppDbContext : IAppDbContext
    {
        public AppDbContext(SQLiteAsyncConnection database)
        {
            Database = database;
        }

        private SQLiteAsyncConnection Database { get; }

        public AsyncTableQuery<InternalValueEntity> InternalValues => Database.Table<InternalValueEntity>();

        public AsyncTableQuery<UserSessionEntity> UserSessions => Database.Table<UserSessionEntity>();

        public async Task RunInTransactionAsync(Action<SQLiteConnection> action)
        {
            await Database.RunInTransactionAsync(action).ConfigureAwait(false);
        }

        public async Task CreateTablesAsync(params Type[] types)
        {
            await Database.CreateTablesAsync(CreateFlags.None, types).ConfigureAwait(false);
        }

        public async Task InsertAsync<T>(T item)
        {
            await Database.InsertAsync(item).ConfigureAwait(false);
        }

        public async Task UpdateAsync<T>(T item)
        {
            await Database.UpdateAsync(item).ConfigureAwait(false);
        }

        public async Task DeleteAllAsync<T>()
        {
            await Database.DeleteAllAsync<T>().ConfigureAwait(false);
        }

        public async Task CloseAsync()
        {
            await Database.CloseAsync().ConfigureAwait(false);
        }
    }
}
