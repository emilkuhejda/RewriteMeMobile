using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
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
        private readonly CancellationTokenSource _cancellationTokenSource;

        public TranscribeRecodingPageViewModel(
            IRecordedItemService recordedItemService,
            IUserSessionService userSessionService,
            IFileItemService fileItemService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(fileItemService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
            _cancellationTokenSource = new CancellationTokenSource();

            PlayerViewModel = new PlayerViewModel();
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            InitializeNavigationItems();

            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() != NavigationMode.New)
                    return;

                RecordedItem = navigationParameters.GetValue<RecordedItem>();
                Name = RecordedItem.FileName;

                var filePath = _recordedItemService.GetAudioPath(RecordedItem);
                var source = File.ReadAllBytes(filePath);
                if (!File.Exists(filePath) || !source.Any())
                {
                    AudioFileIsInvalid = true;
                    await DialogService.AlertAsync(
                        Loc.Text(TranslationKeys.InvalidAudioFileErrorMessage),
                        null,
                        Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);
                    return;
                }

                CanTranscribe = await FileItemService.CanTranscribeAsync().ConfigureAwait(false);

                PlayerViewModel.Load(source);

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
                var filePath = _recordedItemService.GetAudioPath(RecordedItem);
                var mediaFile = new MediaFile
                {
                    Name = Name,
                    Language = SelectedLanguage?.Culture,
                    FileName = RecordedItem.AudioFileName,
                    Source = File.ReadAllBytes(filePath)
                };

                var fileItem = await FileItemService.UploadAsync(mediaFile, _cancellationTokenSource.Token).ConfigureAwait(false);
                await FileItemService.TranscribeAsync(fileItem.Id, fileItem.Language).ConfigureAwait(false);
                await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
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

        protected override void BeforeExecuteCommand()
        {
            PlayerViewModel.Pause();
        }

        protected override void DisposeInternal()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            PlayerViewModel?.Dispose();
        }
    }
}
