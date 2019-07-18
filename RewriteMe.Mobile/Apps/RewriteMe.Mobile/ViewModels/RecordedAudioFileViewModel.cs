using System;
using System.Threading.Tasks;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedAudioFileViewModel : DetailItemViewModel<RecordedAudioFile>
    {
        public RecordedAudioFileViewModel(
            PlayerViewModel playerViewModel,
            RecordedAudioFile recordedAudioFile)
            : base(playerViewModel, recordedAudioFile)
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
