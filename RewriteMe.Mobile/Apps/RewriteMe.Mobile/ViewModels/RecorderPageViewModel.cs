using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.AudioRecorder;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using Xamarin.Cognitive.Speech;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecorderPageViewModel : ViewModelBase
    {
        private const int AudioLengthInSeconds = 15;

        private readonly IRecordedItemService _recordedItemService;
        private readonly IMediaService _mediaService;
        private readonly IList<RecognizedAudioFile> _recognizedAudioFiles;
        private readonly Stopwatch _stopwatch;

        private AudioRecorderService _audioRecorder;
        private SpeechApiClient _speechApiClient;

        private string _text;
        private string _recordingTime;
        private bool _isRecording;
        private bool _isRecordingOnly;
        private bool _isExecuting;

        public RecorderPageViewModel(
            IRecordedItemService recordedItemService,
            IMediaService mediaService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
            _mediaService = mediaService;

            _recognizedAudioFiles = new List<RecognizedAudioFile>();
            _stopwatch = new Stopwatch();

            CanGoBack = true;

            RecordingOnlyClickCommand = new DelegateCommand(ExecuteRecordingOnlyClickCommand, CanExecuteRecordingOnlyClickCommand);
            RecordCommand = new AsyncCommand(ExecuteRecordCommand, CanExecute);
        }

        private RecordedItem CurrentRecordedItem { get; set; }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public string RecordingTime
        {
            get => _recordingTime;
            set => SetProperty(ref _recordingTime, value);
        }

        public bool IsRecording
        {
            get => _isRecording;
            set => SetProperty(ref _isRecording, value);
        }

        public bool IsRecordingOnly
        {
            get => _isRecordingOnly;
            set => SetProperty(ref _isRecordingOnly, value);
        }

        public ICommand RecordingOnlyClickCommand { get; }

        public ICommand RecordCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                _speechApiClient = new SpeechApiClient("471ab4db87064a9db2ad428c64d82b0d", SpeechRegion.WestEurope);

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private bool CanExecuteRecordingOnlyClickCommand()
        {
            return !IsRecording;
        }

        private void ExecuteRecordingOnlyClickCommand()
        {
            IsRecordingOnly = !IsRecordingOnly;
        }

        private bool CanExecute()
        {
            return !_isExecuting;
        }

        private async Task ExecuteRecordCommand()
        {
            _isExecuting = true;

            Device.StartTimer(TimeSpan.FromSeconds(0.5), UpdateTimer);

            Text = string.Empty;

            if (IsRecording)
            {
                await StopRecordingAsync().ConfigureAwait(false);
            }
            else
            {
                await StartRecordingAsync(IsRecordingOnly).ConfigureAwait(false);
            }

            _isExecuting = false;
        }

        private bool UpdateTimer()
        {
            var ts = _stopwatch.Elapsed;
            RecordingTime = $"{ts.Minutes:00}:{ts.Seconds:00}";

            return IsRecording;
        }

        private async Task StartRecordingAsync(bool isRecordingOnly)
        {
            IsRecording = true;

            CurrentRecordedItem = await _recordedItemService.CreateRecordedItemAsync(isRecordingOnly).ConfigureAwait(false);
            await StartRecordingInternalAsync(isRecordingOnly).ConfigureAwait(false);

            _stopwatch.Reset();
            _stopwatch.Start();
        }

        private async Task StartRecordingInternalAsync(bool isRecordingOnly)
        {
            if (_audioRecorder != null)
            {
                _audioRecorder.AudioInputReceived -= OnAudioInputReceived;
                _audioRecorder = null;
            }

            var directoryPath = _recordedItemService.GetDirectoryPath();
            if (isRecordingOnly)
            {
                await StartRecordingWithoutRecognitionAsync(directoryPath).ConfigureAwait(false);
            }
            else
            {
                await StartRecordingWithRecognitionAsync(directoryPath).ConfigureAwait(false);
            }
        }

        private async Task StartRecordingWithoutRecognitionAsync(string directoryPath)
        {
            var fileName = CurrentRecordedItem.AudioFileName;
            var filePath = Path.Combine(directoryPath, fileName);

            _audioRecorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                StopRecordingOnSilence = false,
                FilePath = filePath
            };

            await _audioRecorder.StartRecording().ConfigureAwait(false);
        }

        private async Task StartRecordingWithRecognitionAsync(string directoryPath)
        {
            var fileName = Path.GetTempFileName();
            var filePath = Path.Combine(directoryPath, fileName);

            _audioRecorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                StopRecordingOnSilence = false,
                TotalAudioTimeout = TimeSpan.FromSeconds(AudioLengthInSeconds),
                FilePath = filePath
            };

            _audioRecorder.AudioInputReceived += OnAudioInputReceived;

            var audioRecordTask = await _audioRecorder.StartRecording().ConfigureAwait(false);
            RecognizeAsync(audioRecordTask, filePath, CurrentRecordedItem.Id);
        }

        private async void RecognizeAsync(Task audioRecordTask, string filePath, Guid recordedItemId)
        {
            using (var stream = _audioRecorder.GetAudioFileStream())
            {
                var recordedAudioFile = new RecordedAudioFile
                {
                    Id = Guid.NewGuid(),
                    RecordedItemId = recordedItemId,
                    DateCreated = DateTime.UtcNow
                };

                var recognizedAudioFile = new RecognizedAudioFile
                {
                    RecordedAudioFile = recordedAudioFile,
                    FilePath = filePath,
                    IsRecognizing = true
                };

                _recognizedAudioFiles.Add(recognizedAudioFile);

                var simpleResult = await _speechApiClient
                    .SpeechToTextSimple(stream, _audioRecorder.AudioStreamDetails.SampleRate, audioRecordTask)
                    .ConfigureAwait(false);

                Text += simpleResult.DisplayText;

                recordedAudioFile.Transcript = simpleResult.DisplayText;
                recordedAudioFile.RecognitionSpeechResult = simpleResult;
                recognizedAudioFile.IsRecognizing = false;

                await _recordedItemService.InsertAudioFileAsync(recordedAudioFile).ConfigureAwait(false);
            }
        }

        private async void OnAudioInputReceived(object sender, string e)
        {
            if (!IsRecording)
                return;

            await StartRecordingInternalAsync(IsRecordingOnly).ConfigureAwait(false);
        }

        private async Task StopRecordingAsync()
        {
            IsRecording = false;

            if (_audioRecorder == null)
                return;

            _stopwatch.Stop();
            await _audioRecorder.StopRecording().ConfigureAwait(false);
            await ProcessAudioFiles().ConfigureAwait(false);
        }

        private async Task ProcessAudioFiles()
        {
            using (new OperationMonitor(OperationScope))
            {
                await CheckRecognitionProcess().ConfigureAwait(false);

                RecordedAudioFile previousAudioFile = null;
                foreach (var recognizedAudioFile in _recognizedAudioFiles.OrderBy(x => x.RecordedAudioFile.DateCreated))
                {
                    var filePath = recognizedAudioFile.FilePath;
                    var totalTime = GetTotalTime(filePath);
                    var startTime = previousAudioFile?.EndTime ?? TimeSpan.FromSeconds(0);
                    var endTime = startTime.Add(totalTime);

                    var audioFile = recognizedAudioFile.RecordedAudioFile;
                    audioFile.StartTime = startTime;
                    audioFile.EndTime = endTime;
                    audioFile.TotalTime = totalTime;
                    audioFile.Source = File.ReadAllBytes(filePath);

                    await _recordedItemService.UpdateAudioFileAsync(audioFile).ConfigureAwait(false);
                    File.Delete(filePath);

                    previousAudioFile = audioFile;
                }

                _recognizedAudioFiles.Clear();
            }
        }

        private async Task CheckRecognitionProcess()
        {
            while (true)
            {
                if (_recognizedAudioFiles.All(x => !x.IsRecognizing))
                    break;

                await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            }
        }

        private TimeSpan GetTotalTime(string filePath)
        {
            if (File.Exists(filePath))
                return _mediaService.GetTotalTime(filePath);

            return TimeSpan.FromSeconds(0);
        }

        protected override async void DisposeInternal()
        {
            if (_audioRecorder != null)
            {
                if (IsRecording)
                {
                    await StopRecordingAsync().ConfigureAwait(false);
                }

                _audioRecorder.AudioInputReceived -= OnAudioInputReceived;
                _audioRecorder = null;
            }
        }

        private class RecognizedAudioFile
        {
            public RecordedAudioFile RecordedAudioFile { get; set; }

            public string FilePath { get; set; }

            public bool IsRecognizing { get; set; }
        }
    }
}
