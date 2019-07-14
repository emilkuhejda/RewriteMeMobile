using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedAudioFileViewModel : BindableBase
    {
        private readonly PlayerViewModel _playerViewModel;

        private bool _isReloadCommandVisible;
        private string _transcript;
        private bool _isDirty;

        public event EventHandler IsDirtyChanged;

        public RecordedAudioFileViewModel(
            PlayerViewModel playerViewModel,
            RecordedAudioFile recordedAudioFile)
        {
            _playerViewModel = playerViewModel;

            RecordedAudioFile = recordedAudioFile;
            OperationScope = new AsyncOperationScope();

            if (!string.IsNullOrWhiteSpace(recordedAudioFile.UserTranscript))
            {
                _transcript = recordedAudioFile.Transcript;

                IsReloadCommandVisible = CanExecuteReloadCommand();
            }
            else
            {
                _transcript = recordedAudioFile.Transcript;
            }

            Time = $"{recordedAudioFile.StartTime:mm\\:ss} - {recordedAudioFile.EndTime:mm\\:ss}";

            PlayCommand = new DelegateCommand(ExecutePlayCommand);
            ReloadCommand = new DelegateCommand(ExecuteReloadCommand, CanExecuteReloadCommand);
        }

        public AsyncOperationScope OperationScope { get; }

        public ICommand PlayCommand { get; }

        public ICommand ReloadCommand { get; }

        public bool IsReloadCommandVisible
        {
            get => _isReloadCommandVisible;
            set => SetProperty(ref _isReloadCommandVisible, value);
        }

        public RecordedAudioFile RecordedAudioFile { get; }

        public string Time { get; }

        public string Transcript
        {
            get => _transcript;
            set
            {
                if (SetProperty(ref _transcript, value))
                {
                    IsReloadCommandVisible = CanExecuteReloadCommand();
                    IsDirty = true;
                }
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            private set
            {
                if (SetProperty(ref _isDirty, value))
                {
                    OnIsDirtyChanged();
                }
            }
        }

        private bool CanExecuteReloadCommand()
        {
            return !RecordedAudioFile.Transcript.Equals(Transcript, StringComparison.Ordinal);
        }

        private void ExecutePlayCommand()
        {
            using (new OperationMonitor(OperationScope))
            {
                _playerViewModel.Load(RecordedAudioFile.Source);
            }
        }

        private void ExecuteReloadCommand()
        {
            Transcript = RecordedAudioFile.Transcript;
        }

        private void OnIsDirtyChanged()
        {
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
