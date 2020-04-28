using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
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

        private TranscriptAudioSource TranscriptAudioSource { get; set; }

        private WordComponent CurrentComponent { get; set; }

        private bool IsTranscriptChanged
        {
            get
            {
                if (string.IsNullOrEmpty(DetailItem.UserTranscript))
                    return string.IsNullOrEmpty(DetailItem.Transcript);

                return !DetailItem.Transcript.Equals(DetailItem.UserTranscript, StringComparison.Ordinal);
            }
        }

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
            return IsTranscriptChanged;
        }

        protected override async Task ExecutePlayCommandAsync()
        {
            using (new OperationMonitor(OperationScope))
            {
                TranscriptAudioSource = await _transcriptAudioSourceService.GetAsync(DetailItem.Id).ConfigureAwait(false);
                if (TranscriptAudioSource == null)
                {
                    var errorMessage = _transcribeItemManager.IsRunning
                        ? Loc.Text(TranslationKeys.SynchronizationInProgressErrorMessage)
                        : Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage);

                    await DialogService.AlertAsync(errorMessage).ConfigureAwait(false);
                    return;
                }

                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (TranscriptAudioSource.Source == null || !TranscriptAudioSource.Source.Any())
                {
                    var isSuccess = await _transcriptAudioSourceService.RefreshAsync(TranscriptAudioSource.Id, TranscriptAudioSource.TranscribeItemId, _cancellationToken).ConfigureAwait(false);
                    if (isSuccess)
                    {
                        TranscriptAudioSource = await _transcriptAudioSourceService.GetAsync(DetailItem.Id).ConfigureAwait(false);
                    }
                }

                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (TranscriptAudioSource.Source == null || !TranscriptAudioSource.Source.Any())
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage)).ConfigureAwait(false);
                    return;
                }

                PlayerViewModel.Load(TranscriptAudioSource.Id, TranscriptAudioSource.Source);
                TryStartHighlighting();
                PlayerViewModel.Play();
            }
        }

        protected override void ExecuteReloadCommand()
        {
            Transcript = DetailItem.Transcript;

            TryStartHighlighting();
        }

        protected override void ExecuteEditorUnFocusedCommand()
        {
            TryStartHighlighting();
        }

        private void TryStartHighlighting()
        {
            if (TranscriptAudioSource == null || PlayerViewModel == null || TranscriptAudioSource.Id != PlayerViewModel.SourceIdentifier)
                return;

            if (IsTranscriptChanged)
                return;

            TrySetIsHighlightingEnabled(true);

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
                TrySetIsHighlightingEnabled(false);
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
