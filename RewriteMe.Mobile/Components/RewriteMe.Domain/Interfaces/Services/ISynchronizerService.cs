using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ISynchronizerService
    {
        Task Start();

        void Stop();
    }
}
