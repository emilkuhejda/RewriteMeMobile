﻿using System;
using System.IO;
using System.Threading.Tasks;
using Plugin.AudioRecorder;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using Xamarin.Cognitive.Speech;

namespace RewriteMe.Business.Services
{
    public class RecorderService : IRecorderService
    {
        private readonly IRecordedItemRepository _recordedItemRepository;
        private readonly IRecordedAudioFileRepository _recordedAudioFileRepository;
        private readonly IDirectoryProvider _directoryProvider;

        private AudioRecorderService _recorder;

        private bool _isStopped;

        public RecorderService(
            IRecordedItemRepository recordedItemRepository,
            IRecordedAudioFileRepository recordedAudioFileRepository,
            IDirectoryProvider directoryProvider)
        {
            _recordedItemRepository = recordedItemRepository;
            _recordedAudioFileRepository = recordedAudioFileRepository;
            _directoryProvider = directoryProvider;
        }

        private RecordedItem RecordedItem { get; set; }

        private SpeechApiClient SpeechApiClient { get; set; }

        public async Task<RecordedItem> CreateFileAsync()
        {
            var fileId = Guid.NewGuid();
            //var directory = _directoryProvider.GetPath();
            //var path = Path.Combine(directory, fileId.ToString());
            var path = $"/storage/emulated/0/Download/{fileId}";

            Directory.CreateDirectory(path);

            var recordedItem = new RecordedItem
            {
                Id = fileId,
                FileName = fileId.ToString(),
                Path = path,
                DateCreated = DateTime.UtcNow
            };

            await _recordedItemRepository.InsertAsync(recordedItem).ConfigureAwait(false);
            return recordedItem;
        }

        public bool CanStartRecording()
        {
            return _recorder == null;
        }

        public bool CanResumeRecording()
        {
            return _recorder != null && !_recorder.IsRecording;
        }

        public async void StartRecording(RecordedItem recordedItem, string subscriptionKey)
        {
            RecordedItem = recordedItem;
            SpeechApiClient = new SpeechApiClient(subscriptionKey, SpeechRegion.WestEurope);

            await StartRecordingInternal(recordedItem).ConfigureAwait(false);
        }

        public async void ResumeRecording()
        {
            await StartRecordingInternal(RecordedItem).ConfigureAwait(false);
        }

        private async Task StartRecordingInternal(RecordedItem recordedItem)
        {
            _isStopped = false;

            if (_recorder != null)
            {
                _recorder.AudioInputReceived -= OnAudioInputReceived;
                _recorder = null;
            }

            var filePath = Path.Combine(recordedItem.Path, $"{Guid.NewGuid()}.wav");
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

        public async void StopRecording()
        {
            if (_recorder == null)
                return;

            _isStopped = true;

            await _recorder.StopRecording().ConfigureAwait(false);
        }

        private async void RecognizeAsync(Task audioRecordTask)
        {
            using (var stream = _recorder.GetAudioFileStream())
            {
                var simleResult = await SpeechApiClient
                    .SpeechToTextSimple(stream, _recorder.AudioStreamDetails.SampleRate, audioRecordTask)
                    .ConfigureAwait(false);

                var recordedAudioFile = new RecordedAudioFile
                {
                    Id = Guid.NewGuid(),
                    RecordedItemId = RecordedItem.Id,
                    Path = _recorder.GetAudioFilePath(),
                    Transcript = simleResult.DisplayText,
                    RecognitionSpeechResult = simleResult,
                    DateCreated = DateTime.UtcNow
                };

                await _recordedAudioFileRepository.InsertAsync(recordedAudioFile).ConfigureAwait(false);
            }
        }

        private void OnAudioInputReceived(object sender, string e)
        {
            if (_isStopped)
                return;

            StartRecordingInternal(RecordedItem).ConfigureAwait(false);
        }
    }
}
