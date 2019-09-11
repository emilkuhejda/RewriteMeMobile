using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Messages;
using Xamarin.Forms;

namespace RewriteMe.Business.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly ILastUpdatesService _lastUpdatesService;
        private readonly IDeletedFileItemService _deletedFileItemService;
        private readonly IFileItemService _fileItemService;
        private readonly ITranscribeItemService _transcribeItemService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IInformationMessageService _informationMessageService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IInternalValueService _internalValueService;
        private readonly IConnectivityService _connectivityService;

        public event EventHandler<ProgressEventArgs> InitializationProgress;
        public event EventHandler SynchronizationCompleted;

        private int _totalResourceInitializationTasks;
        private int _resourceInitializationTasksDone;

        public SynchronizationService(
            ILastUpdatesService lastUpdatesService,
            IDeletedFileItemService deletedFileItemService,
            IFileItemService fileItemService,
            ITranscribeItemService transcribeItemService,
            IUserSubscriptionService userSubscriptionService,
            IInformationMessageService informationMessageService,
            IRewriteMeWebService rewriteMeWebService,
            IInternalValueService internalValueService,
            IConnectivityService connectivityService)
        {
            _lastUpdatesService = lastUpdatesService;
            _deletedFileItemService = deletedFileItemService;
            _fileItemService = fileItemService;
            _transcribeItemService = transcribeItemService;
            _userSubscriptionService = userSubscriptionService;
            _informationMessageService = informationMessageService;
            _rewriteMeWebService = rewriteMeWebService;
            _internalValueService = internalValueService;
            _connectivityService = connectivityService;

            _connectivityService.ConnectivityChanged += HandleConnectivityChanged;
        }

        public async Task StartAsync(bool notifyServices = true)
        {
            var isAlive = await _rewriteMeWebService.IsAliveAsync().ConfigureAwait(false);
            if (!isAlive)
                return;

            await _rewriteMeWebService.RefreshTokenIfNeededAsync().ConfigureAwait(false);

            await _lastUpdatesService.InitializeAsync().ConfigureAwait(false);
            if (!_lastUpdatesService.IsConnectionSuccessful)
                return;

            await SendPendingDataAsync().ConfigureAwait(false);

            var updateMethods = new List<Func<Task>>
            {
                UpdateFileItemsAsync,
                DeletedFileItemsSynchronizationAsync,
                DeletedFileItemsTotalTimeSynchronizationAsync,
                UpdateTranscribeItemsAsync,
                UpdateUserSubscriptionAsync,
                UpdateInformationMessageAsync,
                UpdateRecognizedTimeAsync
            };

            _totalResourceInitializationTasks = updateMethods.Count;
            _resourceInitializationTasksDone = 0;

            var tasks = updateMethods.WhenTaskDone(OnInitializationProgress).Select(x => x());
            await Task.WhenAll(tasks).ConfigureAwait(false);

            OnSynchronizationCompleted();

            if (notifyServices)
            {
                NotifyBackgroundServices();
            }
        }

        public void NotifyBackgroundServices()
        {
            MessagingCenter.Send(new StartBackgroundServiceMessage(BackgroundServiceType.TranscribeItem), nameof(BackgroundServiceType.TranscribeItem));
            MessagingCenter.Send(new StartBackgroundServiceMessage(BackgroundServiceType.Synchronizer), nameof(BackgroundServiceType.Synchronizer));
        }

        private void OnInitializationProgress()
        {
            var currentTask = Interlocked.Increment(ref _resourceInitializationTasksDone);
            InitializationProgress?.Invoke(this, new ProgressEventArgs(_totalResourceInitializationTasks, currentTask));
        }

        private async Task SendPendingDataAsync()
        {
            var updateMethods = new List<Func<Task>>
            {
                _deletedFileItemService.SendPendingAsync,
                _transcribeItemService.SendPendingAsync,
                _informationMessageService.SendPendingAsync
            };

            var tasks = updateMethods.Select(x => x()).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task UpdateFileItemsAsync()
        {
            var applicationFileItemUpdateDate = _lastUpdatesService.GetFileItemLastUpdate();

            await _fileItemService.SynchronizationAsync(applicationFileItemUpdateDate).ConfigureAwait(false);
        }

        private async Task DeletedFileItemsSynchronizationAsync()
        {
            var lastFileItemSynchronizationTicks = await _internalValueService.GetValueAsync(InternalValues.FileItemSynchronizationTicks).ConfigureAwait(false);
            var lastFileItemSynchronization = new DateTime(lastFileItemSynchronizationTicks);
            var applicationDeletedFileItemUpdateDate = _lastUpdatesService.GetDeletedFileItemLastUpdate();

            await _deletedFileItemService.SynchronizationAsync(applicationDeletedFileItemUpdateDate, lastFileItemSynchronization).ConfigureAwait(false);
        }

        private async Task DeletedFileItemsTotalTimeSynchronizationAsync()
        {
            await _deletedFileItemService.TotalTimeSynchronizationAsync().ConfigureAwait(false);
        }

        private async Task UpdateTranscribeItemsAsync()
        {
            var applicationTranscribeItemUpdateDate = _lastUpdatesService.GetTranscribeItemLastUpdate();

            await _transcribeItemService.SynchronizationAsync(applicationTranscribeItemUpdateDate).ConfigureAwait(false);
        }

        private async Task UpdateUserSubscriptionAsync()
        {
            var applicationUserSubscriptionUpdateDate = _lastUpdatesService.GetUserSubscriptionLastUpdate();

            await _userSubscriptionService.SynchronizationAsync(applicationUserSubscriptionUpdateDate).ConfigureAwait(false);
        }

        private async Task UpdateInformationMessageAsync()
        {
            var applicationInformationMessageUpdateDate = _lastUpdatesService.GetInformationMessageLastUpdate();

            await _informationMessageService.SynchronizationAsync(applicationInformationMessageUpdateDate).ConfigureAwait(false);
        }

        private async Task UpdateRecognizedTimeAsync()
        {
            await _userSubscriptionService.RecognizedTimeSynchronizationAsync().ConfigureAwait(false);
        }

        private void OnSynchronizationCompleted()
        {
            SynchronizationCompleted?.Invoke(this, EventArgs.Empty);
        }

        private async void HandleConnectivityChanged(object sender, EventArgs e)
        {
            if (!_connectivityService.IsConnected)
                return;

            await SendPendingDataAsync().ConfigureAwait(false);
        }
    }
}
