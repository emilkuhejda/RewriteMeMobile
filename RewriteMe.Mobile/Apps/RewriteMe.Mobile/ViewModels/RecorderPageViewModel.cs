using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecorderPageViewModel : ViewModelBase
    {
        private readonly IRecorderService _recorderService;

        private string _text;
        private string _recordingTime;
        private bool _isRecording;
        private bool _isExecuting;

        public RecorderPageViewModel(
            IRecorderService recorderService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _recorderService = recorderService;

            CanGoBack = true;

            recorderService.AudioTranscribed += OnAudioTranscribed;
            recorderService.StatusChanged += OnStatusChanged;

            RecordCommand = new AsyncCommand(ExecuteRecordCommand, CanExecute);
            StopRecordingCommand = new AsyncCommand(ExecuteStopRecordingCommand, CanExecute);
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

        public ICommand RecordCommand { get; }

        public ICommand StopRecordingCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                UpdateUi();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private bool CanExecute()
        {
            return !_isExecuting;
        }

        private async Task ExecuteRecordCommand()
        {
            _isExecuting = true;

            Device.StartTimer(TimeSpan.FromSeconds(0.5), UpdateTimer);

            if (_recorderService.CanStartRecording())
            {
                var recordedItem = await _recorderService.CreateFileAsync().ConfigureAwait(false);

                await _recorderService.StartRecording(recordedItem, "471ab4db87064a9db2ad428c64d82b0d").ConfigureAwait(false);
            }
            else
            {
                if (_recorderService.CanResumeRecording())
                {
                    await _recorderService.ResumeRecording().ConfigureAwait(false);
                }
                else
                {
                    await _recorderService.StopRecording().ConfigureAwait(false);
                }
            }

            _isExecuting = false;
        }

        private async Task ExecuteStopRecordingCommand()
        {
            _isExecuting = true;

            await _recorderService.StopRecording().ConfigureAwait(false);

            _isExecuting = false;
        }

        private bool UpdateTimer()
        {
            var ts = _recorderService.Time;
            RecordingTime = $"{ts.Minutes:00}:{ts.Seconds:00}";

            return _recorderService.IsRecording;
        }

        private void UpdateUi()
        {
            IsRecording = _recorderService.IsRecording;
        }

        private void OnAudioTranscribed(object sender, AudioTranscribedEventArgs e)
        {
            Text += e.Transcript;
        }

        private void OnStatusChanged(object sender, EventArgs e)
        {
            UpdateUi();
        }

        protected override void DisposeInternal()
        {
            _recorderService.AudioTranscribed -= OnAudioTranscribed;
            _recorderService.StatusChanged -= OnStatusChanged;
            _recorderService.Reset();
        }
    }
}
