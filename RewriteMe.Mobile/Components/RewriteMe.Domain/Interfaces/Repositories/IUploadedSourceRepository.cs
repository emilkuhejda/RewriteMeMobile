using System.Threading.Tasks;
using RewriteMe.Domain.Upload;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IUploadedSourceRepository
    {
        Task AddAsync(UploadedSource uploadedSource);

        Task ClearAsync();
    }
}
