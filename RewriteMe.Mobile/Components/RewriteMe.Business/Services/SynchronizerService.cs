using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using RewriteMe.Common.Utils;
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
        private const string FilesListChangedMethod = "file-list";

        private readonly IUserSessionService _userSessionService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly IAppCenterMetricsService _appCenterMetricsService;
        private readonly IApplicationSettings _applicationSettings;
        private readonly ILogger _logger;

        private readonly object _lockObject = new object();

        private HubConnection _hubConnection;

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

#if DEBUG
            _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_applicationSettings.CacheHubUrl, options =>
                    {
                        options.HttpMessageHandlerFactory = handler =>
                        {
                            return new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true };
                        };
                    })
                    .Build();
#else
            _hubConnection = new HubConnectionBuilder().WithUrl(_applicationSettings.CacheHubUrl).Build();
#endif

            try
            {
                await _hubConnection.StartAsync().ConfigureAwait(false);
                var userId = await _userSessionService.GetUserIdAsync().ConfigureAwait(false);
                _hubConnection.On<Guid, string>($"{RecognitionStateChangedMethod}-{userId}", HandleRecognitionStateChangedMessageAsync);
                _hubConnection.On<Guid>($"{FilesListChangedMethod}-{userId}", HandleFilesListChangedMessageAsync);
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

        private async Task HandleFilesListChangedMessageAsync(Guid fileItemId)
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

                IsRunning = false;
            }

            AsyncHelper.RunSync(() => _hubConnection.StopAsync());

            _logger.Info("Stopping synchronizer service.");
        }

        private void OnUnauthorizedCallOccurred()
        {
            UnauthorizedCallOccurred?.Invoke(this, EventArgs.Empty);
        }
    }
}
