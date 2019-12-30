using System;
using RewriteMe.Domain.Events;

namespace RewriteMe.Domain.Interfaces.Managers
{
    public interface ISynchronizationManager
    {
        event EventHandler<ManagerStateChangedEventArgs> StateChanged;

        event EventHandler UnauthorizedCallOccurred;

        bool IsRunning { get; }

        void Cancel();
    }
}
