using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Common.Utils;
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

            RecordCommand = new DelegateCommand(ExecuteRecordCommand);
            StopRecordingCommand = new DelegateCommand(ExecuteStopRecordingCommand);
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public ICommand RecordCommand { get; }

        public ICommand StopRecordingCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var items = await _recordedItemService.GetAllAsync().ConfigureAwait(false);
            }
        }

        private async void ExecuteRecordCommand()
        {
            var recordedItem = await _recorderService.CreateFileAsync().ConfigureAwait(false);

            _recorderService.StartRecording(recordedItem, "471ab4db87064a9db2ad428c64d82b0d");
        }

        private void ExecuteStopRecordingCommand()
        {
            _recorderService.StopRecording();
        }
    }
}
