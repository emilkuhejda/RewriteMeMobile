using System.Threading.Tasks;
using RewriteMe.Domain.Upload;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUploadedSourceService
    {
        Task AddAsync(UploadedSource uploadedSource);
    }
}
