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
        private readonly CancellationToken _cancellationToken;
        private readonly object _lockObject = new object();

        private IEnumerable<WordComponent> _words;
        private bool _isReloadCommandVisible;
        private string _transcript;
        private bool _isDirty;
        private bool _isHighlightingEnabled;
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
            _cancellationToken = cancellationToken;

            SettingsViewModel = settingsViewModel;
            PlayerViewModel = playerViewModel;
            DialogService = dialogService;
            DetailItem = transcribeItem;
            OperationScope = new AsyncOperationScope();

            IsHighlightingEnabled = false;

            SettingsViewModel.SettingsChanged += HandleSettingsChanged;

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
            
            PlayCommand = new AsyncCommand(ExecutePlayCommandAsync);
            ReloadCommand = new DelegateCommand(ExecuteReloadCommand, CanExecuteReloadCommand);
            EditorUnFocusedCommand = new DelegateCommand(ExecuteEditorUnFocusedCommand, CanExecuteEditorUnFocusedCommand);
        }

        private SettingsViewModel SettingsViewModel { get; }

        private PlayerViewModel PlayerViewModel { get; }

        private IDialogService DialogService { get; }

        public AsyncOperationScope OperationScope { get; }

        public ICommand PlayCommand { get; }

        public ICommand ReloadCommand { get; }

        public ICommand EditorUnFocusedCommand { get; }

        private TranscriptAudioSource TranscriptAudioSource { get; set; }

        private WordComponent CurrentComponent { get; set; }

        public bool IsHighlightingEnabled
        {
            get => _isHighlightingEnabled;
            set => SetProperty(ref _isHighlightingEnabled, value);
        }

        public IEnumerable<WordComponent> Words
        {
            get => _words;
            set => SetProperty(ref _words, value);
        }

        public bool IsReloadCommandVisible
        {
            get => _isReloadCommandVisible;
            set => SetProperty(ref _isReloadCommandVisible, value);
        }

        public TranscribeItem DetailItem { get; }

        public string Time { get; protected set; }

        public string Accuracy { get; protected set; }

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

        private bool IsTranscriptChanged
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DetailItem.Transcript) && string.IsNullOrWhiteSpace(DetailItem.UserTranscript))
                    return false;

                if (string.IsNullOrWhiteSpace(DetailItem.Transcript) && !string.IsNullOrWhiteSpace(DetailItem.UserTranscript))
                    return true;

                if (string.IsNullOrWhiteSpace(DetailItem.UserTranscript))
                    return false;

                return !DetailItem.Transcript.Equals(DetailItem.UserTranscript, StringComparison.Ordinal);
            }
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

        private void ExecuteReloadCommand()
        {
            Transcript = DetailItem.Transcript;

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

        private void HandleSettingsChanged(object sender, EventArgs e)
        {
            if (SettingsViewModel.IsHighlightingEnabled)
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
            if (SettingsViewModel.IsHighlightingEnabled && isHighlightingEnabled && Words != null && Words.Any())
            {
                IsHighlightingEnabled = true;
            }
            else
            {
                IsHighlightingEnabled = false;
            }
        }

        private void SetTranscript(string transcript)
        {
            _transcript = transcript;
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
                PlayerViewModel.Tick -= HandleTick;
                SettingsViewModel.SettingsChanged -= HandleSettingsChanged;
            }

            _disposed = true;
        }
    }
}
