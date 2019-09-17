using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Events;

namespace RewriteMe.Domain.Interfaces.Managers
{
    public interface ITranscribeItemManager
    {
        event EventHandler<ProgressEventArgs> InitializationProgress;

        event EventHandler<ManagerStateChangedEventArgs> StateChanged;

        event EventHandler UnauthorizedCallOccurred;

        bool IsRunning { get; }

        Task SynchronizationAsync();

        void Cancel();
    }
}
