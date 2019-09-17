using System;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class SynchronizerService : ISynchronizerService
    {
        private const int TimeoutSeconds = 30;

        private readonly IFileItemService _fileItemService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly object _lockObject = new object();

        private CancellationTokenSource _cancellationTokenSource;

        public event EventHandler UnauthorizedCallOccurred;

        public SynchronizerService(
            IFileItemService fileItemService,
            ISynchronizationService synchronizationService)
        {
            _fileItemService = fileItemService;
            _synchronizationService = synchronizationService;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public bool IsRunning { get; private set; }

        public async Task StartAsync()
        {
            lock (_lockObject)
            {
                if (IsRunning)
                    return;

                IsRunning = true;
            }

            var anyWaitingForSynchronization = await _fileItemService.AnyWaitingForSynchronizationAsync().ConfigureAwait(false);
            if (anyWaitingForSynchronization)
            {
                await StartInternalAsync().ConfigureAwait(false);
            }
            else
            {
                IsRunning = false;
            }
        }

        public void Cancel()
        {
            lock (_lockObject)
            {
                if (!IsRunning)
                    return;

                _cancellationTokenSource.Cancel();

                IsRunning = false;
            }
        }

        private async Task StartInternalAsync()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var token = _cancellationTokenSource.Token;
                await Task.Delay(TimeSpan.FromSeconds(TimeoutSeconds), token).ConfigureAwait(false);

                token.ThrowIfCancellationRequested();
                var anyWaitingForSynchronization = await _fileItemService.AnyWaitingForSynchronizationAsync().ConfigureAwait(false);
                if (anyWaitingForSynchronization)
                {
                    token.ThrowIfCancellationRequested();
                    await _synchronizationService.StartAsync(false).ConfigureAwait(false);

                    token.ThrowIfCancellationRequested();
                    IsRunning = false;
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (UnauthorizedCallException)
            {
                _cancellationTokenSource.Cancel();

                OnUnauthorizedCallOccurred();
            }
            finally
            {
                if (!IsRunning && !_cancellationTokenSource.IsCancellationRequested)
                {
                    _synchronizationService.NotifyBackgroundServices();
                }
                else
                {
                    IsRunning = false;
                }
            }
        }

        private void OnUnauthorizedCallOccurred()
        {
            UnauthorizedCallOccurred?.Invoke(this, EventArgs.Empty);
        }
    }
}
