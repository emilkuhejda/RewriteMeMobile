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

            InitializeWords(transcribeItem);
        }

        private WordComponent CurrentComponent { get; set; }

        private bool IsTranscriptChanged => DetailItem.Transcript.Equals(DetailItem.UserTranscript, StringComparison.Ordinal);

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
            return !IsTranscriptChanged;
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
                TryStartHighlighting();
                PlayerViewModel.Play();
            }
        }

        protected override void ExecuteReloadCommand()
        {
            Transcript = DetailItem.Transcript;
        }

        protected override void ExecuteEditorUnFocusedCommand()
        {
            TryStartHighlighting();
        }

        private void TryStartHighlighting()
        {
            if (IsTranscriptChanged)
                return;

            TrySetIsHighlightEnabled(true);

            PlayerViewModel.ClearOnStopAction();
            PlayerViewModel.SetOnStopAction(OnStopAction());
            PlayerViewModel.Tick -= HandleTick;
            PlayerViewModel.Tick += HandleTick;
        }

        private void InitializeWords(TranscribeItem transcribeItem)
        {
            var groups = transcribeItem.Alternatives
                .SelectMany(x => x.Words)
                .OrderBy(x => x.StartTimeTicks)
                .ToArray()
                .Split(3);

            var words = new List<WordComponent>();
            foreach (var enumerable in groups)
            {
                var group = enumerable.ToList();
                words.Add(new WordComponent
                {
                    Text = string.Join(" ", group.Select(x => x.Word)),
                    StartTime = group.First().StartTime.TotalSeconds
                });
            }

            Words = words;
        }

        private Action OnStopAction()
        {
            return () =>
            {
                PlayerViewModel.Tick -= HandleTick;
            };
        }

        private void HandleTick(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                var currentPosition = PlayerViewModel.CurrentPosition;
                var currentItem = Words.LastOrDefault(x => currentPosition >= x.StartTime);
                if (currentItem == null)
                    return;

                if (CurrentComponent != null)
                {
                    CurrentComponent.IsHighlighted = false;
                }

                CurrentComponent = currentItem;
                CurrentComponent.IsHighlighted = true;
            }
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();

            PlayerViewModel.Tick -= HandleTick;
        }
    }
}
