using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private const int AudioLengthInSeconds = 15;

        private readonly IRecordedItemService _recordedItemService;
        private readonly IMediaService _mediaService;
        private readonly IList<RecordedAudioFile> _recordedAudioFiles;
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

            _recordedAudioFiles = new List<RecordedAudioFile>();
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

            var directoryPath = _recordedItemService.GetDirectoryPath();
            var fileName = Path.GetTempFileName();
            var filePath = Path.Combine(directoryPath, fileName);
            _recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                StopRecordingOnSilence = false,
                TotalAudioTimeout = TimeSpan.FromSeconds(AudioLengthInSeconds),
                FilePath = filePath
            };

            _recorder.AudioInputReceived += OnAudioInputReceived;

            var audioRecordTask = await _recorder.StartRecording().ConfigureAwait(false);
            RecognizeAsync(audioRecordTask, fileName);

            OnStatusChanged();
        }

        public async Task StopRecording()
        {
            if (_recorder == null)
                return;

            await StopRecordingInternal().ConfigureAwait(false);

            OnStatusChanged();

            await ProcessAudioFiles().ConfigureAwait(false);
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

            _recordedAudioFiles.Clear();

            _stopwatch.Stop();
            _stopwatch.Reset();

            RecordedItem = null;
        }

        private async void RecognizeAsync(Task audioRecordTask, string fileName)
        {
            using (var stream = _recorder.GetAudioFileStream())
            {
                var recordedAudioFile = new RecordedAudioFile
                {
                    Id = Guid.NewGuid(),
                    RecordedItemId = RecordedItem.Id,
                    FileName = fileName,
                    DateCreated = DateTime.UtcNow,
                    IsRecognizing = true
                };
                _recordedAudioFiles.Add(recordedAudioFile);

                var simpleResult = await SpeechApiClient
                    .SpeechToTextSimple(stream, _recorder.AudioStreamDetails.SampleRate, audioRecordTask)
                    .ConfigureAwait(false);

                OnAudioTranscribed(simpleResult.DisplayText);

                recordedAudioFile.Transcript = simpleResult.DisplayText;
                recordedAudioFile.RecognitionSpeechResult = simpleResult;
                recordedAudioFile.IsRecognizing = false;

                await _recordedItemService.InsertAudioFileAsync(recordedAudioFile).ConfigureAwait(false);
            }
        }

        private async Task ProcessAudioFiles()
        {
            while (true)
            {
                if (_recordedAudioFiles.All(x => !x.IsRecognizing))
                    break;

                await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
            }

            if (!_recordedAudioFiles.Any())
                return;

            RecordedAudioFile previousAudioFile = null;
            foreach (var audioFile in _recordedAudioFiles.OrderBy(x => x.DateCreated))
            {
                var directoryPath = _recordedItemService.GetDirectoryPath();
                var filePath = Path.Combine(directoryPath, audioFile.FileName);
                var totalTime = GetTotalTime(filePath);
                var startTime = previousAudioFile?.EndTime ?? TimeSpan.FromSeconds(0);
                var endTime = startTime.Add(totalTime);

                audioFile.StartTime = startTime;
                audioFile.EndTime = endTime;
                audioFile.TotalTime = totalTime;
                audioFile.Source = File.ReadAllBytes(filePath);

                await _recordedItemService.UpdateAudioFileAsync(audioFile).ConfigureAwait(false);
                File.Delete(filePath);

                previousAudioFile = audioFile;
            }

            _recordedAudioFiles.Clear();
        }

        private TimeSpan GetTotalTime(string filePath)
        {
            if (File.Exists(filePath))
                return _mediaService.GetTotalTime(filePath);

            return TimeSpan.FromSeconds(0);
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
