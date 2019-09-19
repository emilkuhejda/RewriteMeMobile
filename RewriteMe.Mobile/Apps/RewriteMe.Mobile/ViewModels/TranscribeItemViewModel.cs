using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribeItemViewModel : DetailItemViewModel<TranscribeItem>
    {
        private readonly ITranscriptAudioSourceService _transcriptAudioSourceService;
        private readonly ITranscribeItemManager _transcribeItemManager;

        public TranscribeItemViewModel(
            ITranscriptAudioSourceService transcriptAudioSourceService,
            ITranscribeItemManager transcribeItemManager,
            IDialogService dialogService,
            PlayerViewModel playerViewModel,
            TranscribeItem transcribeItem)
            : base(playerViewModel, dialogService, transcribeItem)
        {
            _transcriptAudioSourceService = transcriptAudioSourceService;
            _transcribeItemManager = transcribeItemManager;

            if (!string.IsNullOrWhiteSpace(transcribeItem.UserTranscript))
            {
                SetTranscript(transcribeItem.UserTranscript);
                IsReloadCommandVisible = CanExecuteReload();
            }
            else if (transcribeItem.Alternatives != null && transcribeItem.Alternatives.Any())
            {
                var alternative = transcribeItem.Alternatives.First();
                SetTranscript(alternative.Transcript);
            }

            Time = $"{transcribeItem.StartTime:mm\\:ss} - {transcribeItem.EndTime:mm\\:ss}";
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
            if (DetailItem.Alternatives == null || !DetailItem.Alternatives.Any())
                return false;

            var alternative = DetailItem.Alternatives.First();
            return !alternative.Transcript.Equals(DetailItem.UserTranscript, StringComparison.Ordinal);
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

                if (transcriptAudioSource.Source == null || !transcriptAudioSource.Source.Any())
                {
                    var isSuccess = await _transcriptAudioSourceService.RefreshAsync(transcriptAudioSource.Id, transcriptAudioSource.TranscribeItemId, default(CancellationToken)).ConfigureAwait(false);
                    if (isSuccess)
                    {
                        transcriptAudioSource = await _transcriptAudioSourceService.GetAsync(DetailItem.Id).ConfigureAwait(false);
                    }
                }

                if (transcriptAudioSource.Source == null || !transcriptAudioSource.Source.Any())
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage)).ConfigureAwait(false);
                    return;
                }

                PlayerViewModel.Load(transcriptAudioSource.Source);
                PlayerViewModel.Play();
            }
        }

        protected override void ExecuteReloadCommand()
        {
            if (DetailItem == null || !DetailItem.Alternatives.Any())
                return;

            var alternative = DetailItem.Alternatives.First();
            Transcript = alternative.Transcript;
        }
    }
}
