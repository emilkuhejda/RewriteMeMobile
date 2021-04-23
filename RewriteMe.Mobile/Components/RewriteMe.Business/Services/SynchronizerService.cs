using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class SynchronizerService : ISynchronizerService
    {
        private const string RecognitionStateChangedMethod = "recognition-state";
        private const string RecognitionErrorMethod = "recognition-error";
        private const string FilesListChangedMethod = "file-list";

        private readonly IUserSessionService _userSessionService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IAppCenterMetricsService _appCenterMetricsService;
        private readonly IApplicationSettings _applicationSettings;
        private readonly ILogger _logger;

        private readonly object _lockObject = new object();

        private HubConnection _hubConnection;

        public event EventHandler<RecognitionErrorOccurredEventArgs> RecognitionErrorOccurred;
        public event EventHandler UnauthorizedCallOccurred;

        public SynchronizerService(
            IUserSessionService userSessionService,
            ISynchronizationService synchronizationService,
            IAppCenterMetricsService appCenterMetricsService,
            IApplicationSettings applicationSettings,
            ILoggerFactory loggerFactory)
        {
            _userSessionService = userSessionService;
            _synchronizationService = synchronizationService;
            _appCenterMetricsService = appCenterMetricsService;
            _applicationSettings = applicationSettings;
            _logger = loggerFactory.CreateLogger(typeof(SynchronizerService));
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

            _logger.Info("Starting synchronizer service up.");

            var reconnectDelays = new[]
            {
                TimeSpan.FromSeconds(30),
                TimeSpan.FromMinutes(60),
                TimeSpan.FromSeconds(90),
                TimeSpan.FromSeconds(120),
                TimeSpan.FromSeconds(150),
                TimeSpan.FromSeconds(180),
                TimeSpan.FromSeconds(360)
            };

            try
            {
#if DEBUG
                _hubConnection = new HubConnectionBuilder()
                        .WithUrl(_applicationSettings.HubUrl, options =>
                        {
                            options.HttpMessageHandlerFactory = handler =>
                            {
                                return new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true };
                            };
                        })
                        .WithAutomaticReconnect(reconnectDelays)
                        .Build();

#else
            _hubConnection = new HubConnectionBuilder().WithUrl(_applicationSettings.HubUrl).WithAutomaticReconnect(reconnectDelays).Build();
#endif

                await _hubConnection.StartAsync().ConfigureAwait(false);
                var userId = await _userSessionService.GetUserIdAsync().ConfigureAwait(false);
                _hubConnection.On<Guid, string>($"{RecognitionStateChangedMethod}-{userId}", HandleRecognitionStateChangedMessageAsync);
                _hubConnection.On<string>($"{RecognitionErrorMethod}-{userId}", HandleRecognitionErrorMethodAsync);
                _hubConnection.On($"{FilesListChangedMethod}-{userId}", HandleFilesListChangedMessageAsync);
            }
            catch (Exception ex)
            {
                IsRunning = false;

                _appCenterMetricsService.TrackException(ex);
            }
        }

        private async Task HandleRecognitionStateChangedMessageAsync(Guid fileItemId, string recognitionState)
        {
            _logger.Info("Receive recognition state change message.");

            await StartSynchronizationAsync().ConfigureAwait(false);
        }

        private async Task HandleRecognitionErrorMethodAsync(string fileName)
        {
            _logger.Info("Receive transcription error message.");

            OnRecognitionErrorOccurred(fileName);

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task HandleFilesListChangedMessageAsync()
        {
            _logger.Info("Receive file list changed message.");

            await StartSynchronizationAsync().ConfigureAwait(false);
        }

        private async Task StartSynchronizationAsync()
        {
            try
            {
                await _synchronizationService.StartAsync().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (UnauthorizedCallException)
            {
                OnUnauthorizedCallOccurred();
            }
        }

        public void Cancel()
        {
            lock (_lockObject)
            {
                if (!IsRunning)
                    return;

                AsyncHelper.RunSync(async () =>
                {
                    if (_hubConnection != null)
                    {
                        await _hubConnection.StopAsync().ConfigureAwait(false);
                        await _hubConnection.DisposeAsync().ConfigureAwait(false);
                        _hubConnection = null;
                    }
                });

                _logger.Info("Stopping synchronizer service.");

                IsRunning = false;
            }
        }

        private void OnRecognitionErrorOccurred(string fileName)
        {
            RecognitionErrorOccurred?.Invoke(this, new RecognitionErrorOccurredEventArgs(fileName));
        }

        private void OnUnauthorizedCallOccurred()
        {
            UnauthorizedCallOccurred?.Invoke(this, EventArgs.Empty);
        }
    }
}
