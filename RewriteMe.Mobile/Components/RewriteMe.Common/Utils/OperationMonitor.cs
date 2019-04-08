using System;

namespace RewriteMe.Common.Utils
{
    public class OperationMonitor : IDisposable
    {
        private bool _disposed;
        private AsyncOperationScope _host;

        /// <summary>
        /// Initialize a new operation progress and attach to the given monitor.
        /// </summary>
        /// <param name="host">
        /// Monitor to attach
        /// </param>
        public OperationMonitor(AsyncOperationScope host)
        {
            _host = host ?? throw new ArgumentNullException(nameof(host));

            _host.Attach(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// Flag, if <see cref="Dispose()"/> has been called.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_host.IsAttached(this))
                    {
                        _host.Detach(this);
                    }
                    _host = null;
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}
