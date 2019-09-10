using System;

namespace RewriteMe.Domain.Events
{
    public class ManagerStateChangedEventArgs : EventArgs
    {
        public ManagerStateChangedEventArgs(bool isRunning)
        {
            IsRunning = isRunning;
        }

        public bool IsRunning { get; }
    }
}
