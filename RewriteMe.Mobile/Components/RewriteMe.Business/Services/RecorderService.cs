using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Plugin.AudioRecorder;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using Xamarin.Cognitive.Speech;

namespace RewriteMe.Business.Services
{
    public class RecorderService : IRecorderService
    {
        private readonly IRecordedItemService _recordedItemService;
        private readonly IMediaService _mediaService;
        private readonly Stopwatch _stopwatch;

        private AudioRecorderService _recorder;
        private bool _isStopped;

        public event EventHandler<AudioTranscribedEventArgs> AudioTranscribed;
        public event EventHandler StatusChanged;

        public RecorderService(
            IRecordedItemService recordedItemService,
            IMediaService mediaService)
        {
            _recordedItemService = recordedItemService;
            _mediaService = mediaService;

            _stopwatch = new Stopwatch();
        }

        public TimeSpan Time => _stopwatch.Elapsed;

        public bool IsRecording => _recorder != null && !_isStopped;

        private RecordedItem RecordedItem { get; set; }

        private SpeechApiClient SpeechApiClient { get; set; }

        public async Task<RecordedItem> CreateFileAsync()
        {
            var fileId = Guid.NewGuid();
            return await _recordedItemService.CreateRecordedItemAsync(fileId).ConfigureAwait(false);
        }

        public bool CanStartRecording()
        {
            return _recorder == null;
        }

        public bool CanResumeRecording()
        {
            return _recorder != null && !_recorder.IsRecording;
        }

        public async Task StartRecording(RecordedItem recordedItem, string subscriptionKey)
        {
            RecordedItem = recordedItem;
            SpeechApiClient = new SpeechApiClient(subscriptionKey, SpeechRegion.WestEurope);

            await StartRecordingInternal(recordedItem).ConfigureAwait(false);

            _stopwatch.Start();
        }

        public async Task ResumeRecording()
        {
            await StartRecordingInternal(RecordedItem).ConfigureAwait(false);

            _stopwatch.Start();
        }

        private async Task StartRecordingInternal(RecordedItem recordedItem)
        {
            _isStopped = false;

            if (_recorder != null)
            {
                _recorder.AudioInputReceived -= OnAudioInputReceived;
                _recorder = null;
            }

            if (recordedItem == null)
                return;

            var filePath = Path.Combine(recordedItem.Path, $"{Guid.NewGuid()}.wav");
            _recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                StopRecordingOnSilence = false,
                TotalAudioTimeout = TimeSpan.FromSeconds(5),
                FilePath = filePath
            };

            _recorder.AudioInputReceived += OnAudioInputReceived;

            var audioRecordTask = await _recorder.StartRecording().ConfigureAwait(false);
            RecognizeAsync(audioRecordTask);

            OnStatusChanged();
        }

        public async Task StopRecording()
        {
            if (_recorder == null)
                return;

            await StopRecordingInternal().ConfigureAwait(false);

            OnStatusChanged();
        }

        public async Task StopRecordingInternal()
        {
            _isStopped = true;

            await _recorder.StopRecording().ConfigureAwait(false);

            _stopwatch.Stop();
        }

        public async Task Reset()
        {
            if (_recorder != null)
            {
                await StopRecordingInternal().ConfigureAwait(false);
                _recorder = null;
            }

            _stopwatch.Stop();
            _stopwatch.Reset();

            RecordedItem = null;
        }

        private async void RecognizeAsync(Task audioRecordTask)
        {
            using (var stream = _recorder.GetAudioFileStream())
            {
                //var simpleResult = await SpeechApiClient
                //    .SpeechToTextSimple(stream, _recorder.AudioStreamDetails.SampleRate, audioRecordTask)
                //    .ConfigureAwait(false);
                await audioRecordTask.ConfigureAwait(false);
                var simpleResult = new RecognitionSpeechResult { DisplayText = "Text." };

                var recordedAudioFile = new RecordedAudioFile
                {
                    Id = Guid.NewGuid(),
                    RecordedItemId = RecordedItem.Id,
                    Path = _recorder.FilePath,
                    Transcript = simpleResult.DisplayText,
                    RecognitionSpeechResult = simpleResult,
                    DateCreated = DateTime.UtcNow
                };

                OnAudioTranscribed(simpleResult.DisplayText);

                await _recordedItemService.InsertAudioFileAsync(recordedAudioFile).ConfigureAwait(false);
            }
        }

        private void OnAudioInputReceived(object sender, string e)
        {
            if (_isStopped)
                return;

            StartRecordingInternal(RecordedItem).ConfigureAwait(false);
        }

        private void OnAudioTranscribed(string transcript)
        {
            AudioTranscribed?.Invoke(this, new AudioTranscribedEventArgs(transcript));
        }

        private void OnStatusChanged()
        {
            StatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
