using System;
using System.Threading.Tasks;
using RewriteMe.DataAccess.Entities;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

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

        public AsyncTableQuery<FileItemEntity> FileItems => Database.Table<FileItemEntity>();

        public AsyncTableQuery<TranscribeItemEntity> TranscribeItems => Database.Table<TranscribeItemEntity>();

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

        public async Task DeleteAsync(object primaryKey)
        {
            await Database.DeleteAsync(primaryKey).ConfigureAwait(false);
        }

        public async Task DeleteAllAsync<T>() where T : new()
        {
            var items = await Database.GetAllWithChildrenAsync<T>(recursive: true).ConfigureAwait(false);
            await Database.DeleteAllAsync(items, true).ConfigureAwait(false);
        }

        public async Task<T> GetWithChildrenAsync<T>(object primaryKey) where T : new()
        {
            return await Database.GetWithChildrenAsync<T>(primaryKey).ConfigureAwait(false);
        }

        public async Task InsertOrReplaceAsync(object obj)
        {
            await Database.InsertOrReplaceAsync(obj).ConfigureAwait(false);
        }

        public async Task CloseAsync()
        {
            await Database.CloseAsync().ConfigureAwait(false);
        }
    }
}
