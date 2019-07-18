using System;
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

        private TimeSpan _audioTotalTime;

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

            PlayerViewModel = new PlayerViewModel();
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
                    _audioTotalTime = _mediaService.GetTotalTime(filePath);
                    CanTranscribe = await FileItemService.CanTranscribeAsync(_audioTotalTime).ConfigureAwait(false);
                    ReevaluateNavigationItemIconKeys();

                    PlayerViewModel.Load(File.ReadAllBytes(filePath));
                }
                else if (navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var dropDownListViewModel = navigationParameters.GetValue<DropDownListViewModel>();
                    HandleSelectionAsync(dropDownListViewModel);
                }

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        public PlayerViewModel PlayerViewModel { get; }

        protected RecordedItem RecordedItem { get; set; }

        protected override async Task ExecuteTranscribeInternalAsync()
        {
            var result = await DialogService.ConfirmAsync(
                Loc.Text(TranslationKeys.UploadFileItemInfoMessage),
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (result)
            {
                var filePath = Path.Combine(_recordedItemService.GetDirectoryPath(), RecordedItem.AudioFileName);
                using (var fileStream = File.OpenRead(filePath))
                {
                    var mediaFile = new MediaFile()
                    {
                        Name = Name,
                        Language = SelectedLanguage?.Culture,
                        FileName = RecordedItem.AudioFileName,
                        TotalTime = _audioTotalTime,
                        Stream = fileStream
                    };

                    var fileItem = await FileItemService.UploadAsync(mediaFile).ConfigureAwait(false);
                    await FileItemService.TranscribeAsync(fileItem.Id, fileItem.Language).ConfigureAwait(false);
                    await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                }
            }
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

        protected override void DisposeInternal()
        {
            PlayerViewModel?.Dispose();
        }
    }
}
