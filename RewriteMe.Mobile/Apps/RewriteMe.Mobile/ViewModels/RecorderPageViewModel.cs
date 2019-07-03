using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecorderPageViewModel : ViewModelBase
    {
        private readonly IRecorderService _recorderService;
        private readonly IRecordedItemService _recordedItemService;

        private string _text;
        private string _time;
        private string _buttonText;

        public RecorderPageViewModel(
            IRecorderService recorderService,
            IRecordedItemService recordedItemService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _recorderService = recorderService;
            _recordedItemService = recordedItemService;

            CanGoBack = true;

            recorderService.AudioTranscribed += OnAudioTranscribed;
            recorderService.StatusChanged += OnStatusChanged;
            recorderService.Timer.Elapsed += OnElapsed;

            RecordCommand = new DelegateCommand(ExecuteRecordCommand);
            StopRecordingCommand = new DelegateCommand(ExecuteStopRecordingCommand);
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public string Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        public string ButtonText
        {
            get => _buttonText;
            set => SetProperty(ref _buttonText, value);
        }

        public ICommand RecordCommand { get; }

        public ICommand StopRecordingCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                UpdateUi();
                var items = await _recordedItemService.GetAllAsync().ConfigureAwait(false);
            }
        }

        private async void ExecuteRecordCommand()
        {
            if (_recorderService.CanStartRecording())
            {
                var recordedItem = await _recorderService.CreateFileAsync().ConfigureAwait(false);

                _recorderService.StartRecording(recordedItem, "471ab4db87064a9db2ad428c64d82b0d");
            }
            else
            {
                if (_recorderService.CanResumeRecording())
                {
                    _recorderService.ResumeRecording();
                }
                else
                {
                    _recorderService.StopRecording();
                }
            }
        }

        private void ExecuteStopRecordingCommand()
        {
            _recorderService.StopRecording();
        }

        private void UpdateUi()
        {
            if (_recorderService.CanStartRecording())
            {
                ButtonText = "Start";
            }
            else
            {
                if (_recorderService.CanResumeRecording())
                {
                    ButtonText = "Resume";
                }
                else
                {
                    ButtonText = "Stop";
                }
            }
        }

        private void OnAudioTranscribed(object sender, AudioTranscribedEventArgs e)
        {
            Text += e.Transcript;
        }

        private void OnStatusChanged(object sender, EventArgs e)
        {
            UpdateUi();
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            var ts = _recorderService.Time;
            Time = $"{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        }
    }
}
