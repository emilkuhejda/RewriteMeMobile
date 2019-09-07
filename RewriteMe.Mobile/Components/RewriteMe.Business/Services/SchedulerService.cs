using System;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class SchedulerService : ISchedulerService
    {
        private const int TimeoutSeconds = 30;

        private readonly IFileItemService _fileItemService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly object _lockObject = new object();

        private CancellationTokenSource _cancellationTokenSource;

        public SchedulerService(
            IFileItemService fileItemService,
            ISynchronizationService synchronizationService)
        {
            _fileItemService = fileItemService;
            _synchronizationService = synchronizationService;

            _cancellationTokenSource = new CancellationTokenSource();

            _fileItemService.TranscriptionStarted += HandleTranscriptionStarted;
        }

        public bool IsRunning { get; private set; }

        public async void Start()
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

            IsRunning = false;
        }

        public void Stop()
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

            var token = _cancellationTokenSource.Token;
            await Task.Delay(TimeSpan.FromSeconds(TimeoutSeconds)).ConfigureAwait(false);

            if (token.IsCancellationRequested)
                return;

            var anyWaitingForSynchronization = await _fileItemService.AnyWaitingForSynchronizationAsync().ConfigureAwait(false);
            if (anyWaitingForSynchronization)
            {
                await StartSynchronizationAsync(token).ConfigureAwait(false);

                await StartInternalAsync().ConfigureAwait(false);
            }
        }

        private async Task StartSynchronizationAsync(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            await _synchronizationService.StartAsync().ConfigureAwait(false);
        }

        private void HandleTranscriptionStarted(object sender, EventArgs e)
        {
            Start();
        }
    }
}
