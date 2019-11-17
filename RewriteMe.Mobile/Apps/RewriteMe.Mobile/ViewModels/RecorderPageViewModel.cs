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
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Transcription;
using RewriteMe.Resources.Localization;
using Xamarin.Cognitive.Speech;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecorderPageViewModel : ViewModelBase
    {
        private const int AudioDurationInSeconds = 11;
        private const int TotalAudioDurationInMinutes = 5;

        private readonly IRecordedItemService _recordedItemService;
        private readonly IMediaService _mediaService;
        private readonly IScreenService _screenService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IList<RecognizedAudioFile> _recognizedAudioFiles;
        private readonly TimeSpan _totalAudioDuration;
        private readonly Stopwatch _stopwatch;

        private AudioRecorderService _audioRecorder;
        private SpeechApiClient _speechApiClient;

        private SupportedLanguage _selectedLanguage;
        private string _text;
        private string _recordingTime;
        private bool _isRecording;
        private bool _isRecordingOnly;
        private bool _isExecuting;
        private bool _isNotSupportedLanguage;

        public RecorderPageViewModel(
            IRecordedItemService recordedItemService,
            IMediaService mediaService,
            IScreenService screenService,
            IUserSubscriptionService userSubscriptionService,
            IRewriteMeWebService rewriteMeWebService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
            _mediaService = mediaService;
            _screenService = screenService;
            _userSubscriptionService = userSubscriptionService;
            _rewriteMeWebService = rewriteMeWebService;

            _recognizedAudioFiles = new List<RecognizedAudioFile>();
            _totalAudioDuration = TimeSpan.FromMinutes(TotalAudioDurationInMinutes);
            _stopwatch = new Stopwatch();

            CanGoBack = true;
            Languages = SupportedLanguages.All.Where(x => x.IsAzureSupported).ToList();
            SelectedLanguage = SupportedLanguages.EnglishGb;

            RecordingOnlyClickCommand = new DelegateCommand(ExecuteRecordingOnlyClickCommand, CanExecuteRecordingOnlyClickCommand);
            RecordCommand = new AsyncCommand(ExecuteRecordCommand, CanExecute);
        }

        private RecordedItem CurrentRecordedItem { get; set; }

        private SpeechConfiguration Configuration { get; set; }

        public IEnumerable<SupportedLanguage> Languages { get; set; }

        public SupportedLanguage SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

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

            Text = string.Empty;

            if (IsRecording)
            {
                await StopRecordingAsync().ConfigureAwait(false);
            }
            else
            {
                _screenService.DisableIdle();

                _isNotSupportedLanguage = false;
                Configuration = null;

                if (!IsRecordingOnly)
                {
                    if (!AzureSupportedLanguages.IsSupported(SelectedLanguage.Culture))
                    {
                        await DialogService.AlertAsync(Loc.Text(TranslationKeys.LanguageNotSupportedErrorMessage)).ConfigureAwait(false);
                        _isExecuting = false;
                        return;
                    }

                    var isSuccess = await InitializeSpeechConfigurationAsync().ConfigureAwait(false);
                    if (!isSuccess)
                    {
                        _isExecuting = false;
                        return;
                    }

                    var speechRegion = EnumHelper.Parse(Configuration.SpeechRegion, SpeechRegion.WestEurope);
                    _speechApiClient = new SpeechApiClient(Configuration.SubscriptionKey, speechRegion) { RecognitionLanguage = SelectedLanguage.Culture };
                }

                await StartRecordingAsync(IsRecordingOnly).ConfigureAwait(false);
                Device.StartTimer(TimeSpan.FromSeconds(0.5), UpdateTimer);
            }

            _isExecuting = false;
        }

        private async Task<bool> InitializeSpeechConfigurationAsync()
        {
            var remainingTime = await _userSubscriptionService.GetRemainingTimeAsync().ConfigureAwait(false);
            if (remainingTime.Ticks < 0)
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.NotEnoughFreeMinutesInSubscriptionErrorMessage)).ConfigureAwait(false);
                return false;
            }

            Configuration = await GetSpeechConfigurationAsync().ConfigureAwait(false);
            if (Configuration == null)
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.SpeechClientNotInitializedErrorMessage)).ConfigureAwait(false);
                return false;
            }

            if (Configuration.SubscriptionRemainingTime.Ticks < 0)
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.NotEnoughFreeMinutesInSubscriptionErrorMessage)).ConfigureAwait(false);
                return false;
            }

            return true;
        }

        private async Task<SpeechConfiguration> GetSpeechConfigurationAsync()
        {
            using (new OperationMonitor(OperationScope))
            {
                var httpRequestResult = await _rewriteMeWebService.GetSpeechConfigurationAsync().ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    return httpRequestResult.Payload;
                }
            }

            return null;
        }

        private bool UpdateTimer()
        {
            var ts = _stopwatch.Elapsed;
            RecordingTime = $"{ts.Minutes:00}:{ts.Seconds:00}";

            if (!IsRecordingOnly && _audioRecorder != null && _audioRecorder.IsRecording)
            {
                if (ts.Ticks >= _totalAudioDuration.Ticks || ts.Ticks >= Configuration.SubscriptionRemainingTime.Ticks)
                {
                    StopRecordingAsync().ConfigureAwait(false);
                }
            }

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
                TotalAudioTimeout = TimeSpan.FromSeconds(AudioDurationInSeconds),
                SilenceThreshold = 1,
                FilePath = filePath
            };

            _audioRecorder.AudioInputReceived += OnAudioInputReceived;

            var audioRecordTask = await _audioRecorder.StartRecording().ConfigureAwait(false);
            RecognizeAsync(audioRecordTask, filePath, CurrentRecordedItem.Id).FireAndForget();
        }

        private async Task RecognizeAsync(Task audioRecordTask, string filePath, Guid recordedItemId)
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

            using (var stream = _audioRecorder.GetAudioFileStream())
            {
                try
                {
                    var simpleResult = await _speechApiClient
                        .SpeechToTextSimple(stream, _audioRecorder.AudioStreamDetails.SampleRate, audioRecordTask)
                        .ConfigureAwait(false);

                    recordedAudioFile.Transcript = simpleResult.DisplayText;
                    recordedAudioFile.RecognitionSpeechResult = simpleResult;
                }
                catch (Exception)
                {
                    if (_isNotSupportedLanguage)
                        return;

                    _isNotSupportedLanguage = true;
                    await StopRecordingAsync().ConfigureAwait(false);
                    await DialogService.AlertAsync(Loc.Text(TranslationKeys.LanguageNotSupportedErrorMessage)).ConfigureAwait(false);
                    return;
                }

                ReloadText();
            }

            await _recordedItemService.InsertAudioFileAsync(recordedAudioFile).ConfigureAwait(false);
            recognizedAudioFile.IsRecognizing = false;

            await _rewriteMeWebService
                .CreateSpeechResultAsync(recordedAudioFile.Id, Configuration.AudioSampleId, recordedAudioFile.Transcript)
                .ConfigureAwait(false);
        }

        private void ReloadText()
        {
            Text = string.Join(" ", _recognizedAudioFiles.OrderBy(x => x.RecordedAudioFile.DateCreated).Select(x => x.RecordedAudioFile.Transcript));
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

                if (_isNotSupportedLanguage)
                {
                    _recognizedAudioFiles.Clear();
                    return;
                }

                RecordedAudioFile previousAudioFile = null;
                var audioRecordTotalTime = TimeSpan.FromSeconds(0);
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

                    audioRecordTotalTime = audioRecordTotalTime.Add(totalTime);
                    previousAudioFile = audioFile;
                }

                await _userSubscriptionService.SubtractTimeAsync(audioRecordTotalTime).ConfigureAwait(false);

                var models = _recognizedAudioFiles.Select(x => new SpeechResultModel(x.RecordedAudioFile.Id, x.RecordedAudioFile.TotalTime.ToString())).ToList();
                var httpRequestResult = await _rewriteMeWebService.UpdateSpeechResultsAsync(models).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    await _userSubscriptionService.UpdateRemainingTimeAsync(httpRequestResult.Payload.Time).ConfigureAwait(false);
                }

                ReloadText();

                _recognizedAudioFiles.Clear();
            }
        }

        private async Task CheckRecognitionProcess()
        {
            while (true)
            {
                if (_recognizedAudioFiles.All(x => !x.IsRecognizing) || _isNotSupportedLanguage)
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
            _screenService.EnableIdle();

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
