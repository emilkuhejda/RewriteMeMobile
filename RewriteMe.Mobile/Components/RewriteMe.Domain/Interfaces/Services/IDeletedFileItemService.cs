using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IDeletedFileItemService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate, DateTime lastFileItemSynchronization);

        Task TotalTimeSynchronizationAsync();

        Task InsertAsync(DeletedFileItem deletedFileItem);

        Task SendPendingAsync();
    }
}
