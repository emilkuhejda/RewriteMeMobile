using System;
using System.Threading.Tasks;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Transcription;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedAudioFileViewModel : DetailItemViewModel<RecordedAudioFile>
    {
        public RecordedAudioFileViewModel(
            PlayerViewModel playerViewModel,
            IDialogService dialogService,
            RecordedAudioFile recordedAudioFile)
            : base(playerViewModel, dialogService, recordedAudioFile)
        {
            if (!string.IsNullOrWhiteSpace(recordedAudioFile.UserTranscript))
            {
                SetTranscript(recordedAudioFile.UserTranscript);
                IsReloadCommandVisible = CanExecuteReload();
            }
            else
            {
                SetTranscript(recordedAudioFile.Transcript);
            }

            Time = $"{recordedAudioFile.StartTime:mm\\:ss} - {recordedAudioFile.EndTime:mm\\:ss}";
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
            return !DetailItem.Transcript.Equals(Transcript, StringComparison.Ordinal);
        }

        protected override async Task ExecutePlayCommandAsync()
        {
            using (new OperationMonitor(OperationScope))
            {
                if (DetailItem.Source == null)
                {
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.UnableToLoadAudioFile)).ConfigureAwait(false);
                    return;
                }

                PlayerViewModel.Load(DetailItem.Source);
                PlayerViewModel.Play();
            }

            await Task.CompletedTask.ConfigureAwait(false);
        }

        protected override void ExecuteReloadCommand()
        {
            Transcript = DetailItem.Transcript;
        }
    }
}
