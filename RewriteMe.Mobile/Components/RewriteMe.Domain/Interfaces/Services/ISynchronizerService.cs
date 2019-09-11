using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ISynchronizerService
    {
        Task StartAsync();

        void Cancel();
    }
}
