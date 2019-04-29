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
        private readonly IInternalValueService _internalValueService;

        public event EventHandler<ProgressEventArgs> InitializationProgress;

        private int _totalResourceInitializationTasks;
        private int _resourceInitializationTasksDone;

        public SynchronizationService(
            ILastUpdatesService lastUpdatesService,
            IInternalValueService internalValueService)
        {
            _lastUpdatesService = lastUpdatesService;
            _internalValueService = internalValueService;
        }

        public async Task InitializeAsync()
        {
            var updateMethods = new List<Func<Task>>
            {
                UpdateFileItemsAsync,
                UpdateAudioSourcesAsync,
                UpdateTranscribeItemAsync
            };

            _totalResourceInitializationTasks = updateMethods.Count;
            _resourceInitializationTasksDone = 0;

            var tasks = updateMethods.WhenTaskDone(OnInitializationProgress).Select(x => x());
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task<bool> IsFirstTimeDataSyncAsync()
        {
            var fileItemSynchronization = await _internalValueService.GetValueAsync(InternalValues.FileItemSynchronization).ConfigureAwait(false);
            var audioSourceSynchronization = await _internalValueService.GetValueAsync(InternalValues.AudioSourceSynchronization).ConfigureAwait(false);
            var transcribeItemSynchronization = await _internalValueService.GetValueAsync(InternalValues.TranscribeItemSynchronization).ConfigureAwait(false);

            var atLeastOneHasNoData = !fileItemSynchronization.HasValue ||
                                      !audioSourceSynchronization.HasValue ||
                                      !transcribeItemSynchronization.HasValue;

            return atLeastOneHasNoData;
        }

        private void OnInitializationProgress()
        {
            var currentTask = Interlocked.Increment(ref _resourceInitializationTasksDone);
            InitializationProgress?.Invoke(this, new ProgressEventArgs(_totalResourceInitializationTasks, currentTask));
        }

        private async Task UpdateFileItemsAsync()
        {
            var applicationFileItemVersion = _lastUpdatesService.GetFileItemVersion();

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task UpdateAudioSourcesAsync()
        {
            var applicationAudioSourceVersion = _lastUpdatesService.GetAudioSourceVersion();

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task UpdateTranscribeItemAsync()
        {
            var applicationTranscribeItemVersion = _lastUpdatesService.GetTranscribeItemVersion();

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
