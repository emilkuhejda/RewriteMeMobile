using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IDeletedFileItemService
    {
        Task InsertAsync(DeletedFileItem deletedFileItem);

        Task SendPendingAsync();
    }
}
