using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.DataAccess.Entities;
using SQLite;
using SQLiteNetExtensionsAsync.Extensions;

namespace RewriteMe.DataAccess
{
    public class AppDbContext : IAppDbContext
    {
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

        public AppDbContext(SQLiteAsyncConnection database)
        {
            Database = database;
        }

        private SQLiteAsyncConnection Database { get; }

        public AsyncTableQuery<InternalValueEntity> InternalValues => Database.Table<InternalValueEntity>();

        public AsyncTableQuery<UserSessionEntity> UserSessions => Database.Table<UserSessionEntity>();

        public AsyncTableQuery<UserSubscriptionEntity> UserSubscriptions => Database.Table<UserSubscriptionEntity>();

        public AsyncTableQuery<FileItemEntity> FileItems => Database.Table<FileItemEntity>();

        public AsyncTableQuery<DeletedFileItemEntity> DeletedFileItems => Database.Table<DeletedFileItemEntity>();

        public AsyncTableQuery<TranscribeItemEntity> TranscribeItems => Database.Table<TranscribeItemEntity>();

        public AsyncTableQuery<TranscriptAudioSourceEntity> TranscriptAudioSources => Database.Table<TranscriptAudioSourceEntity>();

        public AsyncTableQuery<RecordedItemEntity> RecordedItems => Database.Table<RecordedItemEntity>();

        public AsyncTableQuery<RecordedAudioFileEntity> RecordedAudioFiles => Database.Table<RecordedAudioFileEntity>();

        public AsyncTableQuery<InformationMessageEntity> InformationMessages => Database.Table<InformationMessageEntity>();

        public async Task RunInTransactionAsync(Action<SQLiteConnection> action)
        {
            await SemaphoreSlim.WaitAsync().ConfigureAwait(true);
            try
            {
                await Database.RunInTransactionAsync(action).ConfigureAwait(true);
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }

        public async Task<int> GetVersionNumberAsync()
        {
            return await Database.ExecuteScalarAsync<int>("PRAGMA user_version;").ConfigureAwait(false);
        }

        public async Task UpdateVersionNumberAsync(int versionNumber)
        {
            await Database.ExecuteAsync($"PRAGMA user_version = {versionNumber};").ConfigureAwait(false);
        }

        public async Task CreateTablesAsync(params Type[] types)
        {
            await Database.CreateTablesAsync(CreateFlags.None, types).ConfigureAwait(false);
        }

        public async Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return await Database.GetAsync(predicate).ConfigureAwait(false);
        }

        public async Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return await Database.FindAsync(predicate).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetAllWithChildrenAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return await Database.GetAllWithChildrenAsync(predicate).ConfigureAwait(false);
        }

        public async Task<T> GetWithChildrenAsync<T>(object primaryKey) where T : new()
        {
            return await Database.GetWithChildrenAsync<T>(primaryKey).ConfigureAwait(false);
        }

        public async Task InsertAsync<T>(T item)
        {
            await Database.InsertAsync(item).ConfigureAwait(false);
        }

        public async Task UpdateAsync<T>(T item)
        {
            await Database.UpdateAsync(item).ConfigureAwait(false);
        }

        public async Task UpdateAllAsync(IEnumerable items)
        {
            await Database.UpdateAllAsync(items).ConfigureAwait(false);
        }

        public async Task DeleteAllIdsAsync<T>(IEnumerable<object> primaryKey)
        {
            await Database.DeleteAllIdsAsync<T>(primaryKey).ConfigureAwait(false);
        }

        public async Task DeleteAsync<T>(object primaryKey) where T : new()
        {
            await Database.DeleteAsync<T>(primaryKey).ConfigureAwait(false);
        }

        public async Task DeleteWithChildrenAsync<T>(object primaryKey) where T : new()
        {
            var entity = await GetWithChildrenAsync<T>(primaryKey).ConfigureAwait(false);
            await Database.DeleteAsync(entity, true).ConfigureAwait(false);
        }

        public async Task DeleteAllAsync<T>() where T : new()
        {
            var entities = await Database.GetAllWithChildrenAsync<T>(recursive: true).ConfigureAwait(false);
            await Database.DeleteAllAsync(entities, true).ConfigureAwait(false);
        }

        public async Task DropTable(Type type)
        {
            await Database.DropTableAsync(new TableMapping(type)).ConfigureAwait(false);
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
