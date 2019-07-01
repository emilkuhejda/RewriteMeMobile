using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.AudioRecorder;
using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using Xamarin.Cognitive.Speech;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecorderPageViewModel : ViewModelBase
    {
        private readonly SpeechApiClient _speechApiClient;
        private AudioRecorderService _recorder;

        private bool _isStopped;

        private string _text;

        public RecorderPageViewModel(
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            CanGoBack = true;

            RecordCommand = new AsyncCommand(ExecuteRecordCommandAsync);
            StopRecordingCommand = new AsyncCommand(ExecuteStopRecordingCommandAsync);

            _speechApiClient = new SpeechApiClient("471ab4db87064a9db2ad428c64d82b0d", SpeechRegion.WestEurope);
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public ICommand RecordCommand { get; }

        public ICommand StopRecordingCommand { get; }

        private async Task ExecuteRecordCommandAsync()
        {
            Text = string.Empty;

            await RunRecorder().ConfigureAwait(false);
        }

        private async Task ExecuteStopRecordingCommandAsync()
        {
            if (!_recorder.IsRecording)
                return;

            _isStopped = true;

            await _recorder.StopRecording().ConfigureAwait(false);
        }

        private async Task RunRecorder()
        {
            _isStopped = false;

            if (_recorder != null)
            {
                _recorder.AudioInputReceived -= OnAudioInputReceived;
                _recorder = null;
            }

            var filePath = $"/storage/emulated/0/Download/{DateTime.Now.Ticks}.wav";
            _recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                StopRecordingOnSilence = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(15),
                FilePath = filePath
            };

            _recorder.AudioInputReceived += OnAudioInputReceived;

            var audioRecordTask = await _recorder.StartRecording().ConfigureAwait(false);
            RecognizeAsync(audioRecordTask);
        }

        private async void RecognizeAsync(Task audioRecordTask)
        {
            using (var stream = _recorder.GetAudioFileStream())
            {
                var simleResult = await _speechApiClient
                    .SpeechToTextSimple(stream, _recorder.AudioStreamDetails.SampleRate, audioRecordTask)
                    .ConfigureAwait(false);

                Text += simleResult.DisplayText;
            }
        }

        private async void OnAudioInputReceived(object sender, string e)
        {
            if (_isStopped)
                return;

            await RunRecorder().ConfigureAwait(false);
        }
    }
}
