using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Controls;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribeItemViewModel : BindableBase, IDisposable
    {
        private const int MergeWordsCount = 3;

        private readonly ITranscriptAudioSourceService _transcriptAudioSourceService;
        private readonly ITranscribeItemManager _transcribeItemManager;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly PlayerViewModel _playerViewModel;
        private readonly IDialogService _dialogService;
        private readonly CancellationToken _cancellationToken;
        private readonly object _lockObject = new object();

        private TranscriptAudioSource _transcriptAudioSource;
        private WordComponent _currentComponent;

        private IEnumerable<WordComponent> _words;
        private bool _isHighlightingEnabled;
        private bool _isReloadCommandVisible;
        private string _transcript;
        private bool _isDirty;
        private bool _disposed;

        public event EventHandler IsDirtyChanged;

        public TranscribeItemViewModel(
            ITranscriptAudioSourceService transcriptAudioSourceService,
            ITranscribeItemManager transcribeItemManager,
            SettingsViewModel settingsViewModel,
            PlayerViewModel playerViewModel,
            IDialogService dialogService,
            TranscribeItem transcribeItem,
            CancellationToken cancellationToken)
        {
            _transcriptAudioSourceService = transcriptAudioSourceService;
            _transcribeItemManager = transcribeItemManager;
            _settingsViewModel = settingsViewModel;
            _playerViewModel = playerViewModel;
            _dialogService = dialogService;
            _cancellationToken = cancellationToken;

            TranscribeItem = transcribeItem;
            OperationScope = new AsyncOperationScope();
            IsHighlightingEnabled = false;

            PlayCommand = new AsyncCommand(ExecutePlayCommandAsync);
            ReloadCommand = new DelegateCommand(ExecuteReloadCommand, CanExecuteReloadCommand);
            EditorUnFocusedCommand = new DelegateCommand(ExecuteEditorUnFocusedCommand, CanExecuteEditorUnFocusedCommand);

            _settingsViewModel.SettingsChanged += HandleSettingsChanged;
        }

        public AsyncOperationScope OperationScope { get; set; }

        public ICommand PlayCommand { get; }

        public ICommand ReloadCommand { get; }

        public ICommand EditorUnFocusedCommand { get; }

        public TranscribeItem TranscribeItem { get; }

        public string Time { get; protected set; }

        public string Accuracy { get; protected set; }

        public IEnumerable<WordComponent> Words
        {
            get => _words;
            set => SetProperty(ref _words, value);
        }

        public bool IsHighlightingEnabled
        {
            get => _isHighlightingEnabled;
            set => SetProperty(ref _isHighlightingEnabled, value);
        }

        public bool IsReloadCommandVisible
        {
            get => _isReloadCommandVisible;
            set => SetProperty(ref _isReloadCommandVisible, value);
        }

        public string Transcript
        {
            get => _transcript;
            set
            {
                if (SetProperty(ref _transcript, value))
                {
                    TranscribeItem.UserTranscript = value;
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

        private bool IsTranscriptChanged
        {
            get
            {
                if (string.IsNullOrWhiteSpace(TranscribeItem.Transcript) && string.IsNullOrWhiteSpace(TranscribeItem.UserTranscript))
                    return false;

                if (string.IsNullOrWhiteSpace(TranscribeItem.Transcript) && !string.IsNullOrWhiteSpace(TranscribeItem.UserTranscript))
                    return true;

                if (string.IsNullOrWhiteSpace(TranscribeItem.UserTranscript))
                    return false;

                return !TranscribeItem.Transcript.Equals(TranscribeItem.UserTranscript, StringComparison.Ordinal);
            }
        }

        public void Initialize()
        {
            if (!string.IsNullOrWhiteSpace(TranscribeItem.UserTranscript))
            {
                _transcript = TranscribeItem.UserTranscript;
                IsReloadCommandVisible = CanExecuteReloadCommand();
            }
            else if (!string.IsNullOrWhiteSpace(TranscribeItem.Transcript))
            {
                _transcript = TranscribeItem.Transcript;
            }

            Time = TranscribeItem.TimeRange;
            Accuracy = Loc.Text(TranslationKeys.Accuracy, TranscribeItem.ToAverageConfidence());

            InitializeWords(TranscribeItem);
        }

        private void InitializeWords(TranscribeItem transcribeItem)
        {
            var groups = transcribeItem.Alternatives
                .SelectMany(x => x.Words)
                .OrderBy(x => x.StartTimeTicks)
                .ToArray()
                .Split(MergeWordsCount);

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

        private async Task ExecutePlayCommandAsync()
        {
            using (new OperationMonitor(OperationScope))
            {
                _transcriptAudioSource = await _transcriptAudioSourceService.GetAsync(TranscribeItem.Id).ConfigureAwait(false);
                if (_transcriptAudioSource == null)
                {
                    var errorMessage = _transcribeItemManager.IsRunning
                        ? Loc.Text(TranslationKeys.SynchronizationInProgressErrorMessage)
                        : Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage);

                    await _dialogService.AlertAsync(errorMessage).ConfigureAwait(false);
                    return;
                }

                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (_transcriptAudioSource.Source == null || !_transcriptAudioSource.Source.Any())
                {
                    var isSuccess = await _transcriptAudioSourceService.RefreshAsync(_transcriptAudioSource.Id, _transcriptAudioSource.TranscribeItemId, _cancellationToken).ConfigureAwait(false);
                    if (isSuccess)
                    {
                        _transcriptAudioSource = await _transcriptAudioSourceService.GetAsync(TranscribeItem.Id).ConfigureAwait(false);
                    }
                }

                if (_cancellationToken.IsCancellationRequested)
                    return;

                if (_transcriptAudioSource.Source == null || !_transcriptAudioSource.Source.Any())
                {
                    await _dialogService.AlertAsync(Loc.Text(TranslationKeys.TranscribeAudioSourceNotFoundErrorMessage)).ConfigureAwait(false);
                    return;
                }

                _playerViewModel.Load(_transcriptAudioSource.Id, _transcriptAudioSource.Source);
                TryStartHighlighting();
                _playerViewModel.Play();
            }
        }

        private bool CanExecuteReloadCommand()
        {
            return IsTranscriptChanged;
        }

        private void ExecuteReloadCommand()
        {
            Transcript = TranscribeItem.Transcript;

            TryStartHighlighting();
        }

        private bool CanExecuteEditorUnFocusedCommand()
        {
            return IsHighlightingEnabled;
        }

        private void ExecuteEditorUnFocusedCommand()
        {
            TryStartHighlighting();
        }

        private void TryStartHighlighting()
        {
            if (_transcriptAudioSource == null || _playerViewModel == null || _transcriptAudioSource.Id != _playerViewModel.SourceIdentifier)
                return;

            if (IsTranscriptChanged)
                return;

            TrySetIsHighlightingEnabled(true);

            _playerViewModel.ClearOnStopAction();
            _playerViewModel.SetOnStopAction(OnStopAction());
            _playerViewModel.Tick -= HandleTick;
            _playerViewModel.Tick += HandleTick;
        }

        private Action OnStopAction()
        {
            return () =>
            {
                TrySetIsHighlightingEnabled(false);
                _playerViewModel.Tick -= HandleTick;
            };
        }

        private void HandleTick(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                var currentPosition = _playerViewModel.CurrentPosition;
                var currentItem = Words.LastOrDefault(x => currentPosition >= x.StartTime);
                if (currentItem == null)
                    return;

                if (_currentComponent != null)
                {
                    _currentComponent.IsHighlighted = false;
                }

                _currentComponent = currentItem;
                _currentComponent.IsHighlighted = true;
            }
        }

        private void HandleSettingsChanged(object sender, EventArgs e)
        {
            if (_settingsViewModel.IsHighlightingEnabled)
            {
                TryStartHighlighting();
            }
            else
            {
                TrySetIsHighlightingEnabled(false);
            }
        }

        private void TrySetIsHighlightingEnabled(bool isHighlightingEnabled)
        {
            if (_settingsViewModel.IsHighlightingEnabled && isHighlightingEnabled && Words != null && Words.Any())
            {
                IsHighlightingEnabled = true;
            }
            else
            {
                IsHighlightingEnabled = false;
            }
        }

        private void OnIsDirtyChanged()
        {
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _playerViewModel.Tick -= HandleTick;
                _settingsViewModel.SettingsChanged -= HandleSettingsChanged;
            }

            _disposed = true;
        }
    }
}
