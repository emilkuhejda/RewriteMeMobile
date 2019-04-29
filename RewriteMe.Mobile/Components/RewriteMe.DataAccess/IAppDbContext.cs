using System;
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

        Task InsertAsync<T>(T item);

        Task UpdateAsync<T>(T item);

        Task DeleteAllAsync<T>();

        Task CloseAsync();
    }
}
