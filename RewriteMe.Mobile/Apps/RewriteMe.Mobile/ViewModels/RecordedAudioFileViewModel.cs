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
            RecordedFile = recordedAudioFile;
            OperationScope = new AsyncOperationScope();

            PlayCommand = new AsyncCommand(ExecutePlayCommandAsync);
            ReloadCommand = new DelegateCommand(ExecuteReloadCommand, CanExecuteReloadCommand);
        }

        public ICommand PlayCommand { get; }

        public ICommand ReloadCommand { get; }

        public RecordedAudioFile RecordedFile { get; }

        public AsyncOperationScope OperationScope { get; }

        public string Time { get; protected set; }

        public string Transcript
        {
            get => _transcript;
            set
            {
                if (SetProperty(ref _transcript, value))
                {
                    RecordedFile.UserTranscript = value;
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
                if (string.IsNullOrWhiteSpace(RecordedFile.Transcript) && string.IsNullOrWhiteSpace(Transcript))
                    return false;

                if (string.IsNullOrWhiteSpace(RecordedFile.Transcript) && !string.IsNullOrWhiteSpace(Transcript))
                    return true;

                if (string.IsNullOrWhiteSpace(Transcript))
                    return false;

                return !RecordedFile.Transcript.Equals(Transcript, StringComparison.Ordinal);
            }
        }

        public void Initialize()
        {
            if (!string.IsNullOrWhiteSpace(RecordedFile.UserTranscript))
            {
                _transcript = RecordedFile.UserTranscript;
                IsReloadCommandVisible = CanExecuteReloadCommand();
            }
            else
            {
                _transcript = RecordedFile.Transcript;
            }

            Time = RecordedFile.TimeRange;
        }

        private bool CanExecuteReloadCommand()
        {
            return IsTranscriptChanged;
        }

        private async Task ExecutePlayCommandAsync()
        {
            using (new OperationMonitor(OperationScope))
            {
                if (RecordedFile.Source == null)
                {
                    await _dialogService.AlertAsync(Loc.Text(TranslationKeys.UnableToLoadAudioFile)).ConfigureAwait(false);
                    return;
                }

                _playerViewModel.Load(RecordedFile.Source);
                _playerViewModel.Play();
            }
        }

        private void ExecuteReloadCommand()
        {
            Transcript = RecordedFile.Transcript;
        }

        private void OnIsDirtyChanged()
        {
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
