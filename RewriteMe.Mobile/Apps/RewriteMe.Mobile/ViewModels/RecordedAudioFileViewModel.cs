using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Mobile.Commands;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedAudioFileViewModel : BindableBase
    {
        private readonly PlayerViewModel _playerViewModel;
        private readonly IDialogService _dialogService;

        private bool _isReloadCommandVisible;
        private string _transcript;
        private bool _isDirty;

        public event EventHandler IsDirtyChanged;

        public RecordedAudioFileViewModel(
            PlayerViewModel playerViewModel,
            IDialogService dialogService,
            RecordedAudioFile recordedAudioFile)
        {
            _playerViewModel = playerViewModel;
            _dialogService = dialogService;
            DetailItem = recordedAudioFile;
            OperationScope = new AsyncOperationScope();

            PlayCommand = new AsyncCommand(ExecutePlayCommandAsync);
            ReloadCommand = new DelegateCommand(ExecuteReloadCommand, CanExecuteReloadCommand);

            if (!string.IsNullOrWhiteSpace(recordedAudioFile.UserTranscript))
            {
                SetTranscript(recordedAudioFile.UserTranscript);
                IsReloadCommandVisible = CanExecuteReload();
            }
            else
            {
                SetTranscript(recordedAudioFile.Transcript);
            }

            Time = recordedAudioFile.TimeRange;
        }

        public ICommand PlayCommand { get; }

        public ICommand ReloadCommand { get; }

        public RecordedAudioFile DetailItem { get; }

        public AsyncOperationScope OperationScope { get; }

        public string Time { get; protected set; }

        public string Transcript
        {
            get => _transcript;
            set
            {
                if (SetProperty(ref _transcript, value))
                {
                    OnTranscriptChanged(value);
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

        public bool IsReloadCommandVisible
        {
            get => _isReloadCommandVisible;
            set => SetProperty(ref _isReloadCommandVisible, value);
        }

        private bool IsTranscriptChanged
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DetailItem.Transcript) && string.IsNullOrWhiteSpace(Transcript))
                    return false;

                if (string.IsNullOrWhiteSpace(DetailItem.Transcript) && !string.IsNullOrWhiteSpace(Transcript))
                    return true;

                if (string.IsNullOrWhiteSpace(Transcript))
                    return false;

                return !DetailItem.Transcript.Equals(Transcript, StringComparison.Ordinal);
            }
        }

        protected void SetTranscript(string transcript)
        {
            _transcript = transcript;
        }

        private void OnTranscriptChanged(string transcript)
        {
            DetailItem.UserTranscript = transcript;
        }

        private bool CanExecuteReloadCommand()
        {
            return CanExecuteReload();
        }

        private bool CanExecuteReload()
        {
            return IsTranscriptChanged;
        }

        private async Task ExecutePlayCommandAsync()
        {
            using (new OperationMonitor(OperationScope))
            {
                if (DetailItem.Source == null)
                {
                    await _dialogService.AlertAsync(Loc.Text(TranslationKeys.UnableToLoadAudioFile)).ConfigureAwait(false);
                    return;
                }

                _playerViewModel.Load(DetailItem.Source);
                _playerViewModel.Play();
            }
        }

        private void ExecuteReloadCommand()
        {
            Transcript = DetailItem.Transcript;
        }

        private void OnIsDirtyChanged()
        {
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
