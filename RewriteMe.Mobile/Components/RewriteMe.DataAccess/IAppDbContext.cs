using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RewriteMe.DataAccess.Entities;
using SQLite;

namespace RewriteMe.DataAccess
{
    public interface IAppDbContext
    {
        AsyncTableQuery<InternalValueEntity> InternalValues { get; }

        AsyncTableQuery<UserSessionEntity> UserSessions { get; }

        AsyncTableQuery<FileItemEntity> FileItems { get; }

        AsyncTableQuery<DeletedFileItemEntity> DeletedFileItems { get; }

        AsyncTableQuery<TranscribeItemEntity> TranscribeItems { get; }

        AsyncTableQuery<TranscriptAudioSourceEntity> TranscriptAudioSources { get; }

        AsyncTableQuery<RecordedItemEntity> RecordedItems { get; }

        AsyncTableQuery<RecordedAudioFileEntity> RecordedAudioFiles { get; }

        AsyncTableQuery<InformationMessageEntity> InformationMessages { get; }

        Task RunInTransactionAsync(Action<SQLiteConnection> action);

        Task<int> GetVersionNumberAsync();

        Task UpdateVersionNumberAsync(int versionNumber);

        Task CreateTablesAsync(params Type[] types);

        Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : new();

        Task<T> FindAsync<T>(Expression<Func<T, bool>> predicate) where T : new();

        Task<IEnumerable<T>> GetAllWithChildrenAsync<T>(Expression<Func<T, bool>> predicate) where T : new();

        Task<T> GetWithChildrenAsync<T>(object primaryKey) where T : new();

        Task InsertAsync<T>(T item);

        Task UpdateAsync<T>(T item);

        Task UpdateAllAsync(IEnumerable items);

        Task DeleteAllIdsAsync<T>(IEnumerable<object> primaryKey);

        Task DeleteAsync<T>(object primaryKey) where T : new();

        Task DeleteWithChildrenAsync<T>(object primaryKey) where T : new();

        Task DeleteAllAsync<T>() where T : new();

        Task DropTable(Type type);

        Task InsertOrReplaceAsync(object obj);

        Task CloseAsync();
    }
}
