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
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedDetailPageViewModel : ViewModelBase
    {
        private readonly IRecordedItemService _recordedItemService;
        private readonly IMediaService _mediaService;
        private readonly IEmailTask _emailTask;

        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private ISimpleAudioPlayer _audioPlayer;
        private IList<AudioFile> _audioFiles;
        private AudioFile _currentAudioFile;
        private string _text;
        private string _position;
        private double _duration = 1;
        private double _audioCurrentProgress;
        private bool _isPlaying;
        private bool _isDirty;

        public RecordedDetailPageViewModel(
            IRecordedItemService recordedItemService,
            IMediaService mediaService,
            IEmailTask emailTask,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
            _mediaService = mediaService;
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

        public string Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public double Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public double AudioCurrentProgress
        {
            get => _audioCurrentProgress;
            set
            {
                if (SetProperty(ref _audioCurrentProgress, value))
                {
                    if (_audioPlayer != null && _audioPlayer.IsPlaying)
                    {
                        _audioPlayer.Seek(value);
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

        private TimeSpan TotalTime { get; set; }

        private ActionBarTileViewModel SendTileItem { get; set; }

        private ActionBarTileViewModel SaveTileItem { get; set; }

        public ICommand DeleteCommand { get; }

        public ICommand StopPlayCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                RecordedItem = navigationParameters.GetValue<RecordedItem>();

                InitializePlaylist();
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
                _audioPlayer.Pause();
            }
            else if (_audioPlayer != null)
            {
                _audioPlayer.Play();
            }
            else
            {
                PlayAudioFile();
            }

            IsPlaying = !IsPlaying;
            RefreshNavigationButtons();

            Device.StartTimer(TimeSpan.FromSeconds(0.5), UpdatePosition);
        }

        private bool UpdatePosition()
        {
            var currentAudioFile = _currentAudioFile;
            var currentPosition = TimeSpan.FromSeconds((int)_audioPlayer.CurrentPosition).Add(currentAudioFile.Offset);

            Position = $"{currentPosition:mm\\:ss} / {TotalTime:mm\\:ss}";

            _audioCurrentProgress = _audioPlayer.CurrentPosition + currentAudioFile.Offset.Seconds;
            RaisePropertyChanged(nameof(AudioCurrentProgress));

            return IsPlaying;
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
            if (_currentAudioFile.IsLast)
            {
                IsPlaying = false;
                return;
            }

            PlayAudioFile();
        }

        private void InitializePlaylist()
        {
            _audioFiles = new List<AudioFile>();

            var totalTime = TimeSpan.FromSeconds(0);
            var directoryPath = _recordedItemService.GetAudioFilePath(RecordedItem.Id.ToString());
            var directoryInfo = new DirectoryInfo(directoryPath);
            var files = directoryInfo.GetFiles().OrderBy(x => x.CreationTimeUtc);
            if (files.Any())
            {
                var lastItem = files.Last();
                foreach (var file in files)
                {
                    var filePath = file.FullName;
                    var audioFileTime = _mediaService.GetTotalTime(filePath);
                    var isLast = file == lastItem;
                    _audioFiles.Add(new AudioFile(filePath, audioFileTime)
                    {
                        IsLast = isLast,
                        Offset = totalTime
                    });

                    totalTime = totalTime.Add(audioFileTime);
                }
            }

            Duration = totalTime.Seconds;
            TotalTime = totalTime;
        }

        private AudioFile GetNextAudioFile()
        {
            if (_currentAudioFile == null)
                return (_currentAudioFile = _audioFiles.First());

            var index = _audioFiles.IndexOf(_currentAudioFile);
            if (index + 1 < _audioFiles.Count)
                return (_currentAudioFile = _audioFiles[index + 1]);

            return (_currentAudioFile = _audioFiles.First());
        }

        private void PlayAudioFile()
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.PlaybackEnded -= HandlePlaybackEnded;
                _audioPlayer = null;
            }

            var currentAudioFile = GetNextAudioFile();
            _audioPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            _audioPlayer.PlaybackEnded += HandlePlaybackEnded;
            _audioPlayer.Load(currentAudioFile.FilePath);
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

        private class AudioFile
        {
            public AudioFile(string filePath, TimeSpan totalTime)
            {
                FilePath = filePath;
                TotalTime = totalTime;
            }

            public string FilePath { get; }

            public TimeSpan TotalTime { get; }

            public TimeSpan Offset { get; set; }

            public bool IsLast { get; set; }
        }
    }
}
