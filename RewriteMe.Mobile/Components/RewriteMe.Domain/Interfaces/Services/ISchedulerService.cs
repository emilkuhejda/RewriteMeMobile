using System;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ISchedulerService
    {
        event EventHandler SynchronizationCompleted;

        void StartAsync();

        void Stop();
    }
}
