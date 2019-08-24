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
        private readonly IDeletedFileItemService _deletedFileItemService;
        private readonly IFileItemService _fileItemService;
        private readonly ITranscribeItemService _transcribeItemService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IInternalValueService _internalValueService;

        public event EventHandler<ProgressEventArgs> InitializationProgress;

        private int _totalResourceInitializationTasks;
        private int _resourceInitializationTasksDone;

        public SynchronizationService(
            ILastUpdatesService lastUpdatesService,
            IDeletedFileItemService deletedFileItemService,
            IFileItemService fileItemService,
            ITranscribeItemService transcribeItemService,
            IUserSubscriptionService userSubscriptionService,
            IRewriteMeWebService rewriteMeWebService,
            IInternalValueService internalValueService)
        {
            _lastUpdatesService = lastUpdatesService;
            _deletedFileItemService = deletedFileItemService;
            _fileItemService = fileItemService;
            _transcribeItemService = transcribeItemService;
            _userSubscriptionService = userSubscriptionService;
            _rewriteMeWebService = rewriteMeWebService;
            _internalValueService = internalValueService;
        }

        public async Task InitializeAsync()
        {
            var isAlive = await _rewriteMeWebService.IsAliveAsync().ConfigureAwait(false);
            if (!isAlive)
                return;

            await _rewriteMeWebService.RefreshTokenIfNeededAsync().ConfigureAwait(false);

            await _lastUpdatesService.InitializeAsync().ConfigureAwait(false);
            if (!_lastUpdatesService.IsConnectionSuccessful)
                return;

            await SendPendingDeletedFileItems().ConfigureAwait(false);

            var updateMethods = new List<Func<Task>>
            {
                UpdateFileItemsAsync,
                DeletedFileItemsSynchronizationAsync,
                DeletedFileItemsTotalTimeSynchronizationAsync,
                UpdateTranscribeItemsAsync,
                UpdateUserSubscriptionAsync,
                UpdateRecognizedTimeAsync
            };

            _totalResourceInitializationTasks = updateMethods.Count;
            _resourceInitializationTasksDone = 0;

            var tasks = updateMethods.WhenTaskDone(OnInitializationProgress).Select(x => x());
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private void OnInitializationProgress()
        {
            var currentTask = Interlocked.Increment(ref _resourceInitializationTasksDone);
            InitializationProgress?.Invoke(this, new ProgressEventArgs(_totalResourceInitializationTasks, currentTask));
        }

        private async Task SendPendingDeletedFileItems()
        {
            await _deletedFileItemService.SendPendingAsync().ConfigureAwait(false);
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

        private async Task UpdateRecognizedTimeAsync()
        {
            await _userSubscriptionService.RecognizedTimeSynchronizationAsync().ConfigureAwait(false);
        }
    }
}
