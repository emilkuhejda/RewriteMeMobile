using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Managers
{
    public class TranscribeItemManager : SynchronizationManager, ITranscribeItemManager
    {
        private readonly ITranscriptAudioSourceService _transcriptAudioSourceService;
        private readonly ITranscribeItemRepository _transcribeItemRepository;
        private readonly object _lockObject = new object();

        private int _totalResourceInitializationTasks;
        private int _resourceInitializationTasksDone;

        public event EventHandler<ProgressEventArgs> InitializationProgress;

        public TranscribeItemManager(
            ITranscriptAudioSourceService transcriptAudioSourceService,
            ITranscribeItemRepository transcribeItemRepository)
        {
            _transcriptAudioSourceService = transcriptAudioSourceService;
            _transcribeItemRepository = transcribeItemRepository;
        }

        public async Task SynchronizationAsync()
        {
            lock (_lockObject)
            {
                if (IsRunning)
                    return;

                IsRunning = true;
            }

            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();

            try
            {
                await SynchronizationInternalAsync(CancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (UnauthorizedCallException)
            {
                CancellationTokenSource.Cancel();

                OnUnauthorizedCallOccurred();
            }
            finally
            {
                IsRunning = false;
            }
        }

        public async Task SynchronizationInternalAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var itemsToUpdate = await _transcribeItemRepository.GetAllForAudioSourceSynchronizationAsync().ConfigureAwait(false);
            var transcribeItemsToUpdate = itemsToUpdate.ToList();
            _totalResourceInitializationTasks = transcribeItemsToUpdate.Count;
            _resourceInitializationTasksDone = 0;

            if (!transcribeItemsToUpdate.Any())
                return;

            cancellationToken.ThrowIfCancellationRequested();

            var transcribeItems = transcribeItemsToUpdate.Select(x => x.Id).ToArray().Split(10);
            bool isSuccess = true;
            foreach (var transcribeItemIds in transcribeItems)
            {
                var updateMethods = new List<Func<Task<bool>>>();
                foreach (var transcribeItem in transcribeItemIds)
                {
                    updateMethods.Add(() => _transcriptAudioSourceService.SynchronizeAsync(transcribeItem, cancellationToken));
                    cancellationToken.ThrowIfCancellationRequested();
                }

                var tasks = updateMethods.WhenTaskDone(OnInitializationProgress).Select(x => x());
                var result = await Task.WhenAll(tasks).ConfigureAwait(false);

                isSuccess &= result.All(x => x);
            }

            if (!isSuccess)
                return;

            await SynchronizationInternalAsync(cancellationToken).ConfigureAwait(false);
        }

        private void OnInitializationProgress()
        {
            var currentTask = Interlocked.Increment(ref _resourceInitializationTasksDone);
            OnProgressEventArgs(_totalResourceInitializationTasks, currentTask);
        }

        private void OnProgressEventArgs(int totalSteps, int stepsDone)
        {
            InitializationProgress?.Invoke(this, new ProgressEventArgs(totalSteps, stepsDone));
        }
    }
}
