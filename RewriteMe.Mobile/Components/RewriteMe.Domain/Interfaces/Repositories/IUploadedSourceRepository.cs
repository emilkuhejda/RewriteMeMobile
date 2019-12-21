using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Upload;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IUploadedSourceRepository
    {
        Task<UploadedSource> GetFirstAsync();

        Task AddAsync(UploadedSource uploadedSource);

        Task DeleteAsync(Guid uploadedSourceId);

        Task ClearAsync();
    }
}
