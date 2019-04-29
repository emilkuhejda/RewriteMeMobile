using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IFileItemRepository
    {
        Task<IEnumerable<FileItem>> GetAllAsync();

        Task UpdateAsync(IEnumerable<FileItem> fileItems);
    }
}
