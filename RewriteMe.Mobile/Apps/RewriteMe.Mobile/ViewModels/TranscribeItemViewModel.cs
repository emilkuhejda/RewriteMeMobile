using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Mobile.Commands;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribeItemViewModel : BindableBase
    {
        private readonly ITranscriptAudioSourceService _transcriptAudioSourceService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IDialogService _dialogService;
        private readonly PlayerViewModel _playerViewModel;

        private bool _isReloadCommandVisible;
        private string _userTranscript;
        private bool _isDirty;

        public event EventHandler IsDirtyChanged;

        public TranscribeItemViewModel(
            ITranscriptAudioSourceService transcriptAudioSourceService,
            IRewriteMeWebService rewriteMeWebService,
            IDialogService dialogService,
            PlayerViewModel playerViewModel,
            TranscribeItem transcribeItem)
        {
            _transcriptAudioSourceService = transcriptAudioSourceService;
            _rewriteMeWebService = rewriteMeWebService;
            _dialogService = dialogService;
            _playerViewModel = playerViewModel;

            TranscribeItem = transcribeItem;
            OperationScope = new AsyncOperationScope();

            if (!string.IsNullOrWhiteSpace(transcribeItem.UserTranscript))
            {
                _userTranscript = transcribeItem.UserTranscript;

                IsReloadCommandVisible = CanExecuteReloadCommand();
            }
            else if (transcribeItem.Alternatives != null && transcribeItem.Alternatives.Any())
            {
                var alternative = transcribeItem.Alternatives.First();
                _userTranscript = alternative.Transcript;
            }

            Time = $"{transcribeItem.StartTime:mm\\:ss} - {transcribeItem.EndTime:mm\\:ss}";

            PlayCommand = new AsyncCommand(ExecutePlayCommandAsync);
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

        public TranscribeItem TranscribeItem { get; }

        public string Time { get; }

        public string UserTranscript
        {
            get => _userTranscript;
            set
            {
                if (SetProperty(ref _userTranscript, value))
                {
                    TranscribeItem.UserTranscript = _userTranscript;
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
            if (TranscribeItem.Alternatives == null || !TranscribeItem.Alternatives.Any())
                return false;

            var alternative = TranscribeItem.Alternatives.First();
            return !alternative.Transcript.Equals(TranscribeItem.UserTranscript);
        }

        private async Task ExecutePlayCommandAsync()
        {
            using (new OperationMonitor(OperationScope))
            {
                var transcribeItemId = TranscribeItem.Id ?? Guid.Empty;
                var transcriptAudioSource = await _transcriptAudioSourceService.GetAsync(transcribeItemId).ConfigureAwait(false);
                var source = transcriptAudioSource?.Source;

                if (transcriptAudioSource == null)
                {
                    var httpRequestResult = await _rewriteMeWebService.GetTranscribeAudioSourceAsync(transcribeItemId).ConfigureAwait(false);
                    if (httpRequestResult.State == HttpRequestState.Success)
                    {
                        source = httpRequestResult.Payload;

                        var audioSource = new TranscriptAudioSource
                        {
                            Id = Guid.NewGuid(),
                            TranscribeItemId = transcribeItemId,
                            Source = source
                        };

                        await _transcriptAudioSourceService.InsertAsync(audioSource).ConfigureAwait(false);
                    }
                }

                if (source == null)
                {
                    await _dialogService.AlertAsync(Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage)).ConfigureAwait(false);
                    return;
                }

                _playerViewModel.Load(source);
            }
        }

        private void ExecuteReloadCommand()
        {
            if (TranscribeItem == null || !TranscribeItem.Alternatives.Any())
                return;

            var alternative = TranscribeItem.Alternatives.First();
            UserTranscript = alternative.Transcript;
        }

        private void OnIsDirtyChanged()
        {
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
