using System.Threading.Tasks;
using RewriteMe.Domain.Upload;

namespace RewriteMe.Domain.Interfaces.Managers
{
    public interface IFileItemSourceUploader : ISynchronizationManager
    {
        Task UploadAsync();

        UploadedFile CurrentUploadedFile { get; }
    }
}
