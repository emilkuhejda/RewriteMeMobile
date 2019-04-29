using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IFileItemService
    {
        Task SynchronizationAsync(int? applicationFileItemVersion);
    }
}
