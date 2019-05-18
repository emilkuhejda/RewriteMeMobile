using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IDeletedFileItemRepository
    {
        Task InsertAsync(DeletedFileItem deletedFileItem);

        Task<IEnumerable<DeletedFileItem>> GetAllAsync();

        Task ClearAsync();
    }
}
