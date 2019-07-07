using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Messaging;
using Plugin.SimpleAudioPlayer;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;
using RewriteMe.Resources.Utils;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedDetailPageViewModel : ViewModelBase
    {
        private readonly IRecordedItemService _recordedItemService;
        private readonly IEmailTask _emailTask;

        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private ISimpleAudioPlayer _audioPlayer;
        private Queue<string> _audioFiles;
        private string _text;
        private bool _isPlaying;
        private bool _isDirty;

        public RecordedDetailPageViewModel(
            IRecordedItemService recordedItemService,
            IEmailTask emailTask,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
            _emailTask = emailTask;

            CanGoBack = true;

            PropertyChanged += HandlePropertyChanged;

            DeleteCommand = new AsyncCommand(ExecuteDeleteCommandAsync);
            StopPlayCommand = new DelegateCommand(ExecuteStopPlayCommand);
        }

        private RecordedItem RecordedItem { get; set; }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        public string Text
        {
            get => _text;
            set
            {
                if (SetProperty(ref _text, value))
                {
                    if (!OperationScope.IsBusy)
                    {
                        IsDirty = true;
                    }
                }
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => SetProperty(ref _isPlaying, value);
        }

        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }

        private ActionBarTileViewModel SendTileItem { get; set; }

        private ActionBarTileViewModel SaveTileItem { get; set; }

        public ICommand DeleteCommand { get; }

        public ICommand StopPlayCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                RecordedItem = navigationParameters.GetValue<RecordedItem>();

                ReloadPlaylist();
                InitializeTranscriptions();
                InitializeNavigation();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private void InitializeTranscriptions()
        {
            if (string.IsNullOrWhiteSpace(RecordedItem.UserTranscript))
            {
                var transcriptions = RecordedItem.AudioFiles
                    .OrderBy(x => x.DateCreated)
                    .Where(x => !string.IsNullOrWhiteSpace(x.Transcript))
                    .Select(x => x.Transcript);
                Text = string.Join(" ", transcriptions);
            }
            else
            {
                Text = RecordedItem.UserTranscript;
            }
        }

        private void InitializeNavigation()
        {
            SendTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Send),
                IsEnabled = CanExecuteSendCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Disabled.svg",
                SelectedCommand = new DelegateCommand(ExecuteSendCommand, CanExecuteSendCommand)
            };

            SaveTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Save),
                IsEnabled = CanExecuteSaveCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSaveCommandAsync, CanExecuteSaveCommand)
            };

            NavigationItems = new[] { SendTileItem, SaveTileItem };
        }

        private bool CanExecuteSendCommand()
        {
            return !IsPlaying && _emailTask.CanSendEmail && !string.IsNullOrWhiteSpace(Text);
        }

        private void ExecuteSendCommand()
        {
            ThreadHelper.InvokeOnUiThread(SendEmailInternal);
        }

        private void SendEmailInternal()
        {
            _emailTask.SendEmail(
                subject: RecordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat),
                message: Text);
        }

        private bool CanExecuteSaveCommand()
        {
            return !IsPlaying && IsDirty;
        }

        private async Task ExecuteSaveCommandAsync()
        {
            RecordedItem.UserTranscript = Text;
            await _recordedItemService.UpdateAsync(RecordedItem).ConfigureAwait(false);

            IsDirty = false;
        }

        private async Task ExecuteDeleteCommandAsync()
        {
            var title = RecordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat);
            var result = await DialogService.ConfirmAsync(
                Loc.Text(TranslationKeys.PromptDeleteFileItemMessage, title),
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (result)
            {
                using (new OperationMonitor(OperationScope))
                {
                    await _recordedItemService.DeleteRecordedItemAsync(RecordedItem.Id).ConfigureAwait(false);
                    await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                }
            }
        }

        private void ExecuteStopPlayCommand()
        {
            if (!_audioFiles.Any())
                return;

            if (IsPlaying)
            {
                _audioPlayer.Stop();
            }
            else
            {
                var path = _audioFiles.Dequeue();
                PlayAudioFile(path);
            }

            IsPlaying = !IsPlaying;
            RefreshNavigationButtons();
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsDirty))
            {
                RefreshNavigationButtons();
            }
        }

        private void RefreshNavigationButtons()
        {
            ThreadHelper.InvokeOnUiThread(() =>
            {
                SendTileItem.IsEnabled = CanExecuteSendCommand();
                SaveTileItem.IsEnabled = CanExecuteSaveCommand();
            });
        }

        private void HandlePlaybackEnded(object sender, EventArgs e)
        {
            if (!_audioFiles.Any())
            {
                IsPlaying = false;
                ReloadPlaylist();
                return;
            }

            var path = _audioFiles.Dequeue();
            PlayAudioFile(path);
        }

        private void ReloadPlaylist()
        {
            _audioFiles = new Queue<string>();

            var directoryPath = _recordedItemService.GetAudioFilePath(RecordedItem.Id.ToString());
            var directoryInfo = new DirectoryInfo(directoryPath);
            var files = directoryInfo.GetFiles().OrderBy(x => x.CreationTimeUtc);
            if (files.Any())
            {
                files.ForEach(x => _audioFiles.Enqueue(x.FullName));
            }
        }

        private void PlayAudioFile(string path)
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.PlaybackEnded -= HandlePlaybackEnded;
                _audioPlayer = null;
            }

            _audioPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            _audioPlayer.PlaybackEnded += HandlePlaybackEnded;
            _audioPlayer.Load(path);
            _audioPlayer.Play();
        }

        protected override void DisposeInternal()
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.Stop();
                _audioPlayer.PlaybackEnded -= HandlePlaybackEnded;
                _audioPlayer.Dispose();
                _audioPlayer = null;
            }

            PropertyChanged -= HandlePropertyChanged;
        }
    }
}
