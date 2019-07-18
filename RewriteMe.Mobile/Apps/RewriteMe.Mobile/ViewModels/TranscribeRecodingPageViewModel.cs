using System.IO;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribeRecodingPageViewModel : TranscribeBaseViewModel
    {
        private readonly IRecordedItemService _recordedItemService;
        private readonly IMediaService _mediaService;

        public TranscribeRecodingPageViewModel(
            IRecordedItemService recordedItemService,
            IMediaService mediaService,
            IFileItemService fileItemService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(fileItemService, dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
            _mediaService = mediaService;
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            InitializeNavigationItems();

            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    RecordedItem = navigationParameters.GetValue<RecordedItem>();
                    Title = RecordedItem.FileName;
                    Name = RecordedItem.FileName;

                    var filePath = Path.Combine(_recordedItemService.GetDirectoryPath(), RecordedItem.AudioFileName);
                    var audioTotalTime = _mediaService.GetTotalTime(filePath);
                    CanTranscribe = await FileItemService.CanTranscribeAsync(audioTotalTime).ConfigureAwait(false);
                    ReevaluateNavigationItemIconKeys();
                }
                else if (navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var dropDownListViewModel = navigationParameters.GetValue<DropDownListViewModel>();
                    HandleSelectionAsync(dropDownListViewModel);
                }

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        protected RecordedItem RecordedItem { get; set; }

        protected override async Task ExecuteTranscribeInternalAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        protected override async Task ExecuteDeleteInternalAsync()
        {
            var title = RecordedItem.FileName;
            var result = await DialogService.ConfirmAsync(
                Loc.Text(TranslationKeys.PromptDeleteFileItemMessage, title),
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (result)
            {
                using (new OperationMonitor(OperationScope))
                {
                    await _recordedItemService.DeleteRecordedItemAsync(RecordedItem).ConfigureAwait(false);
                    await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
