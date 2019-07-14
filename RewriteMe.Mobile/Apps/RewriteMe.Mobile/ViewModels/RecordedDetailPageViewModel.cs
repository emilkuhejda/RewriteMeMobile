using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Resources.Localization;
using RewriteMe.Resources.Utils;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedDetailPageViewModel : DetailBaseViewModel<RecordedAudioFile>
    {
        private readonly IRecordedItemService _recordedItemService;

        public RecordedDetailPageViewModel(
            IRecordedItemService recordedItemService,
            IEmailTask emailTask,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(emailTask, dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
        }

        private RecordedItem RecordedItem { get; set; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    RecordedItem = navigationParameters.GetValue<RecordedItem>();

                    DetailItems?.ForEach(x => x.IsDirtyChanged -= HandleIsDirtyChanged);
                    DetailItems = RecordedItem.AudioFiles.OrderBy(x => x.DateCreated).Select(CreateDetailItemViewModel).ToList();

                    NotAvailableData = !DetailItems.Any();
                }

                NavigationItems = CreateNavigation();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private DetailItemViewModel<RecordedAudioFile> CreateDetailItemViewModel(RecordedAudioFile detailItem)
        {
            var viewModel = new RecordedAudioFileViewModel(PlayerViewModel, detailItem);
            viewModel.IsDirtyChanged += HandleIsDirtyChanged;

            return viewModel;
        }

        protected override void SendEmailInternal()
        {
            var message = new StringBuilder();
            foreach (var recordedAudioFile in DetailItems)
            {
                message.AppendLine(recordedAudioFile.Transcript);
                message.AppendLine(recordedAudioFile.Time);
                message.AppendLine();
            }

            EmailTask.SendEmail(
                subject: RecordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat),
                message: message.ToString());
        }

        protected override bool CanExecuteSaveCommand()
        {
            return DetailItems.Any(x => x.IsDirty);
        }

        protected override async Task ExecuteSaveCommandAsync()
        {
            var recordedAudioFileToSave = DetailItems.Where(x => x.IsDirty).Select(x => x.DetailItem);

            await _recordedItemService.UpdateAudioFilesAsync(recordedAudioFileToSave).ConfigureAwait(false);
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        protected override async Task ExecuteDeleteCommandAsync()
        {
            var title = RecordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat);
            var result = await DialogService.ConfirmAsync(
                Loc.Text(TranslationKeys.PromptDeleteFileItemMessage, title),
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (result)
            {
                using (new OperationMonitor(OperationScope))
                {
                    await _recordedItemService.DeleteRecordedItemAsync(RecordedItem.Id).ConfigureAwait(false);
                    await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
