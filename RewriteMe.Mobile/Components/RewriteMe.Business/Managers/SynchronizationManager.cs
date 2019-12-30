using System;
using System.Threading;
using RewriteMe.Domain.Events;

namespace RewriteMe.Business.Managers
{
    public abstract class SynchronizationManager
    {
        private bool _isRunning;

        public event EventHandler<ManagerStateChangedEventArgs> StateChanged;
        public event EventHandler UnauthorizedCallOccurred;

        protected CancellationTokenSource CancellationTokenSource { get; set; }

        public bool IsRunning
        {
            get => _isRunning;
            protected set
            {
                _isRunning = value;
                OnStateChanged(value);
            }
        }

        public void Cancel()
        {
            if (CancellationTokenSource != null)
            {
                CancellationTokenSource.Token.ThrowIfCancellationRequested();
                CancellationTokenSource.Cancel();
                CancellationTokenSource.Dispose();
                CancellationTokenSource = null;
            }
        }

        private void OnStateChanged(bool isRunning)
        {
            StateChanged?.Invoke(this, new ManagerStateChangedEventArgs(isRunning));
        }

        protected void OnUnauthorizedCallOccurred()
        {
            UnauthorizedCallOccurred?.Invoke(this, EventArgs.Empty);
        }
    }
}
