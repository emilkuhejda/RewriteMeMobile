using System;
using System.Collections.Concurrent;
using Prism.Mvvm;

namespace RewriteMe.Common.Utils
{
    public class AsyncOperationScope : BindableBase
    {
        private readonly ConcurrentDictionary<OperationMonitor, bool> _operationMonitorSet =
            new ConcurrentDictionary<OperationMonitor, bool>();

        /// <summary>
        /// Flag, if there is a currently monitored operation.
        /// </summary>
        public bool IsBusy => !_operationMonitorSet.IsEmpty;

        /// <summary>
        /// Check whether the given operation monitor is attached.
        /// </summary>
        /// <param name="operationMonitor">
        /// The operation monitor to check.
        /// </param>
        /// <returns>
        /// <c>true</c>: is attached; <c>false</c>: otherwise
        /// </returns>
        public bool IsAttached(OperationMonitor operationMonitor)
        {
            if (operationMonitor == null)
            {
                throw new ArgumentNullException(nameof(operationMonitor));
            }

            return _operationMonitorSet.ContainsKey(operationMonitor);
        }

        /// <summary>
        /// Attach the given operation to the monitor.
        /// </summary>
        /// <param name="operation">
        /// Operation to attach
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Operation monitor not idle
        /// </exception>
        public void Attach(OperationMonitor operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            _operationMonitorSet.TryAdd(operation, true);
            RaisePropertyChanged(nameof(IsBusy));
        }

        /// <summary>
        /// Detach the given operation from the monitor.
        /// </summary>
        /// <param name="operation">
        /// Operation to detach
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Operation monitor idle
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Given operation not the same as the currently monitored operation
        /// </exception>
        public void Detach(OperationMonitor operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (!_operationMonitorSet.TryRemove(operation, out var _))
            {
                throw new InvalidOperationException("operation to detach must be currently monitored");
            }

            RaisePropertyChanged(nameof(IsBusy));
        }
    }
}
