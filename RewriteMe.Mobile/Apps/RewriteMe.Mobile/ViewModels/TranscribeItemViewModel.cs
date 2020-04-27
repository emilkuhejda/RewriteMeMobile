using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi;
using RewriteMe.Mobile.Controls;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribeItemViewModel : DetailItemViewModel<TranscribeItem>
    {
        private readonly ITranscriptAudioSourceService _transcriptAudioSourceService;
        private readonly ITranscribeItemManager _transcribeItemManager;
        private readonly CancellationToken _cancellationToken;
        private readonly object _lockObject = new object();

        public TranscribeItemViewModel(
            ITranscriptAudioSourceService transcriptAudioSourceService,
            ITranscribeItemManager transcribeItemManager,
            PlayerViewModel playerViewModel,
            IDialogService dialogService,
            TranscribeItem transcribeItem,
            CancellationToken cancellationToken)
            : base(playerViewModel, dialogService, transcribeItem)
        {
            _transcriptAudioSourceService = transcriptAudioSourceService;
            _transcribeItemManager = transcribeItemManager;
            _cancellationToken = cancellationToken;

            Words = transcribeItem.Alternatives
                .SelectMany(x => x.Words)
                .OrderBy(x => x.StartTimeTicks)
                .Select(x => new WordComponent
                {
                    Text = x.Word,
                    EndTime = x.EndTime.TotalSeconds
                }).ToList();
            List = new LinkedList<WordComponent>(Words);

            if (!string.IsNullOrWhiteSpace(transcribeItem.UserTranscript))
            {
                SetTranscript(transcribeItem.UserTranscript);
                IsReloadCommandVisible = CanExecuteReload();
            }
            else if (!string.IsNullOrWhiteSpace(transcribeItem.Transcript))
            {
                SetTranscript(transcribeItem.Transcript);
            }

            Time = transcribeItem.TimeRange;
            Accuracy = Loc.Text(TranslationKeys.Accuracy, transcribeItem.ToAverageConfidence());
        }

        private LinkedListNode<WordComponent> CurrentComponent { get; set; }

        private LinkedList<WordComponent> List { get; }

        protected override void OnTranscriptChanged(string transcript)
        {
            DetailItem.UserTranscript = transcript;
        }

        protected override bool CanExecuteReloadCommand()
        {
            return CanExecuteReload();
        }

        private bool CanExecuteReload()
        {
            if (string.IsNullOrWhiteSpace(DetailItem.Transcript))
                return false;

            return !DetailItem.Transcript.Equals(DetailItem.UserTranscript, StringComparison.Ordinal);
        }

        protected override async Task ExecutePlayCommandAsync()
        {
            using (new OperationMonitor(OperationScope))
            {
                var transcriptAudioSource = await _transcriptAudioSourceService.GetAsync(DetailItem.Id).ConfigureAwait(false);
                if (transcriptAudioSource == null)
                {
                    var errorMessage = _transcribeItemManager.IsRunning
                        ? Loc.Text(TranslationKeys.SynchronizationInProgressErrorMessage)
                        : Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage);

                    await DialogService.AlertAsync(errorMessage).ConfigureAwait(false);
                    return;
                }

                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (transcriptAudioSource.Source == null || !transcriptAudioSource.Source.Any())
                {
                    var isSuccess = await _transcriptAudioSourceService.RefreshAsync(transcriptAudioSource.Id, transcriptAudioSource.TranscribeItemId, _cancellationToken).ConfigureAwait(false);
                    if (isSuccess)
                    {
                        transcriptAudioSource = await _transcriptAudioSourceService.GetAsync(DetailItem.Id).ConfigureAwait(false);
                    }
                }

                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (transcriptAudioSource.Source == null || !transcriptAudioSource.Source.Any())
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage)).ConfigureAwait(false);
                    return;
                }

                PlayerViewModel.Load(transcriptAudioSource.Source);

                TrySetIsHighlightEnabled(true);

                PlayerViewModel.Tick -= HandleTick;
                PlayerViewModel.Tick += HandleTick;
                Words.ForEach(x => x.IsHighlighted = false);
                CurrentComponent = List.First;
                CurrentComponent.Value.IsHighlighted = true;

                PlayerViewModel.Play();
            }
        }

        protected override void ExecuteReloadCommand()
        {
            Transcript = DetailItem.Transcript;
        }

        private void HandleTick(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                var current = CurrentComponent.Value;
                var currentPosition = PlayerViewModel.CurrentPosition;
                if (currentPosition <= current.EndTime)
                    return;

                while (currentPosition > current.EndTime)
                {
                    current.IsHighlighted = false;
                    CurrentComponent = CurrentComponent.Next;
                    if (CurrentComponent == null)
                        break;

                    current = CurrentComponent.Value;
                    current.IsHighlighted = true;
                }
            }
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();

            PlayerViewModel.Tick -= HandleTick;
        }
    }
}
