using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Events;

namespace RewriteMe.Domain.Interfaces.Managers
{
    public interface ITranscribeItemManager : ISynchronizationManager
    {
        event EventHandler<ProgressEventArgs> InitializationProgress;

        Task SynchronizationAsync();
    }
}
