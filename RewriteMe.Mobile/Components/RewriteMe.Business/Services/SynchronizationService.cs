using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly ILastUpdatesService _lastUpdatesService;
        private readonly IFileItemService _fileItemService;
        private readonly ITranscribeItemService _transcribeItemService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ISubscriptionProductService _subscriptionProductService;
        private readonly IInternalValueService _internalValueService;

        public event EventHandler<ProgressEventArgs> InitializationProgress;

        private int _totalResourceInitializationTasks;
        private int _resourceInitializationTasksDone;

        public SynchronizationService(
            ILastUpdatesService lastUpdatesService,
            IFileItemService fileItemService,
            ITranscribeItemService transcribeItemService,
            IUserSubscriptionService userSubscriptionService,
            ISubscriptionProductService subscriptionProductService,
            IInternalValueService internalValueService)
        {
            _lastUpdatesService = lastUpdatesService;
            _fileItemService = fileItemService;
            _transcribeItemService = transcribeItemService;
            _userSubscriptionService = userSubscriptionService;
            _subscriptionProductService = subscriptionProductService;
            _internalValueService = internalValueService;
        }

        public async Task InitializeAsync()
        {
            var updateMethods = new List<Func<Task>>
            {
                UpdateFileItemsAsync,
                UpdateTranscribeItemsAsync,
                UpdateUserSubscriptionAsync,
                UpdateSubscriptionProductAsync
            };

            _totalResourceInitializationTasks = updateMethods.Count;
            _resourceInitializationTasksDone = 0;

            var tasks = updateMethods.WhenTaskDone(OnInitializationProgress).Select(x => x());
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task<bool> IsFirstTimeDataSyncAsync()
        {
            var lastFileItemSynchronization = await _internalValueService.GetValueAsync(InternalValues.FileItemSynchronization).ConfigureAwait(false);
            var lastTranscribeItemSynchronization = await _internalValueService.GetValueAsync(InternalValues.TranscribeItemSynchronization).ConfigureAwait(false);

            var atLeastOneHasNoData = lastFileItemSynchronization == default(DateTime) || lastTranscribeItemSynchronization == default(DateTime);
            return atLeastOneHasNoData;
        }

        private void OnInitializationProgress()
        {
            var currentTask = Interlocked.Increment(ref _resourceInitializationTasksDone);
            InitializationProgress?.Invoke(this, new ProgressEventArgs(_totalResourceInitializationTasks, currentTask));
        }

        private async Task UpdateFileItemsAsync()
        {
            var applicationFileItemUpdateDate = _lastUpdatesService.GetFileItemLastUpdate();

            await _fileItemService.SynchronizationAsync(applicationFileItemUpdateDate).ConfigureAwait(false);
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

        private async Task UpdateSubscriptionProductAsync()
        {
            var applicationSubscriptionProductUpdateDate = _lastUpdatesService.GetSubscriptionProductLastUpdate();

            await _subscriptionProductService.SynchronizationAsync(applicationSubscriptionProductUpdateDate).ConfigureAwait(false);
        }
    }
}
