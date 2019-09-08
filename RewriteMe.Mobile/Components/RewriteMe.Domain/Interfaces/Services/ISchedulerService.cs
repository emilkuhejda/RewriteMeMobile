using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ISchedulerService
    {
        Task Start();

        void Stop();
    }
}
