using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Managers
{
    public interface IFileItemSourceUploader : ISynchronizationManager
    {
        Task UploadAsync();
    }
}
