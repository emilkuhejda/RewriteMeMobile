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

        AsyncTableQuery<TranscribeItemEntity> TranscribeItems { get; }

        Task RunInTransactionAsync(Action<SQLiteConnection> action);

        Task CreateTablesAsync(params Type[] types);

        Task<T> GetAsync<T>(Expression<Func<T, bool>> predicate) where T : new();

        Task<IEnumerable<T>> GetAllWithChildrenAsync<T>(Expression<Func<T, bool>> predicate) where T : new();

        Task<T> GetWithChildrenAsync<T>(object primaryKey) where T : new();

        Task InsertAsync<T>(T item);

        Task UpdateAsync<T>(T item);

        Task UpdateAllAsync(IEnumerable items);

        Task DeleteAsync(object primaryKey);

        Task DeleteAllAsync<T>() where T : new();

        Task InsertOrReplaceAsync(object obj);

        Task CloseAsync();
    }
}
