using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Messages;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.Upload;
using RewriteMe.Domain.WebApi;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class TranscribeRecodingPageViewModel : TranscribeBaseViewModel
    {
        private readonly IRecordedItemService _recordedItemService;
        private readonly IUploadedSourceService _uploadedSourceService;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public TranscribeRecodingPageViewModel(
            IRecordedItemService recordedItemService,
            IUploadedSourceService uploadedSourceService,
            IUserSessionService userSessionService,
            IRewriteMeWebService rewriteMeWebService,
            IFileItemService fileItemService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(rewriteMeWebService, fileItemService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
            _uploadedSourceService = uploadedSourceService;
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

            if (!result)
                return;

            var mediaFile = CreateMediaFile();
            var fileItem = await FileItemService.CreateAsync(mediaFile, _cancellationTokenSource.Token).ConfigureAwait(false);
            var uploadedSource = CreateUploadedSource(fileItem, mediaFile);

            await _uploadedSourceService.AddAsync(uploadedSource).ConfigureAwait(false);
            MessagingCenter.Send(new StartBackgroundServiceMessage(BackgroundServiceType.UploadFileItem), nameof(BackgroundServiceType.UploadFileItem));

            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        private MediaFile CreateMediaFile()
        {
            var filePath = _recordedItemService.GetAudioPath(RecordedItem);
            return new MediaFile
            {
                Name = Name,
                Language = SelectedLanguage?.Culture,
                FileName = RecordedItem.AudioFileName,
                Source = File.ReadAllBytes(filePath)
            };
        }

        private UploadedSource CreateUploadedSource(FileItem fileItem, MediaFile mediaFile)
        {
            return new UploadedSource
            {
                Id = Guid.NewGuid(),
                FileItemId = fileItem.Id,
                Language = fileItem.Language,
                Source = mediaFile.Source,
                IsTranscript = true,
                DateCreated = DateTime.UtcNow
            };
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
