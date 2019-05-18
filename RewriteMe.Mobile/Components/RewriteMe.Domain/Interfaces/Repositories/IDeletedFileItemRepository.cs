using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IDeletedFileItemRepository
    {
        Task InsertAsync(DeletedFileItem deletedFileItem);

        Task<IEnumerable<DeletedFileItem>> GetAllAsync();

        Task<TimeSpan> GetProcessedFilesTotalTimeAsync();

        Task ClearAsync();
    }
}
