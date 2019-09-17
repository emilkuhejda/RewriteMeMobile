using System;
using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ISynchronizerService
    {
        event EventHandler UnauthorizedCallOccurred;

        Task StartAsync();

        void Cancel();
    }
}
