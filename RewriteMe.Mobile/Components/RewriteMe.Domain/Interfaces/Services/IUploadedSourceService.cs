using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Upload;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUploadedSourceService
    {
        Task<UploadedSource> GetFirstAsync();

        Task AddAsync(UploadedSource uploadedSource);

        Task DeleteAsync(Guid uploadedSourceId);
    }
}
