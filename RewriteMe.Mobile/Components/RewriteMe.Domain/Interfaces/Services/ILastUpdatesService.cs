using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ILastUpdatesService
    {
        Task InitializeAsync();

        int? GetFileItemVersion();

        int? GetAudioSourceVersion();

        int? GetTranscribeItemVersion();
    }
}
