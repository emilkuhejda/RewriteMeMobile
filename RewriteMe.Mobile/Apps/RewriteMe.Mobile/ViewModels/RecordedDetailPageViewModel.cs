﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
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
            IInternalValueService internalValueService,
            IEmailService emailService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(emailService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;

            SettingsViewModel = new SettingsViewModel(internalValueService);
        }

        public SettingsViewModel SettingsViewModel { get; set; }

        private RecordedItem RecordedItem { get; set; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                await SettingsViewModel.InitializeAsync().ConfigureAwait(false);

                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    var recordedItem = navigationParameters.GetValue<RecordedItem>();
                    RecordedItem = await _recordedItemService.GetAsync(recordedItem.Id).ConfigureAwait(false);

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
            var viewModel = new RecordedAudioFileViewModel(SettingsViewModel, PlayerViewModel, DialogService, detailItem);
            viewModel.IsDirtyChanged += HandleIsDirtyChanged;

            return viewModel;
        }

        protected override async Task SendEmailInternal()
        {
            var message = new StringBuilder();
            foreach (var recordedAudioFile in DetailItems)
            {
                message.AppendLine(recordedAudioFile.Time);
                message.AppendLine(recordedAudioFile.Transcript);
                message.AppendLine();
            }

            await EmailService.SendAsync(string.Empty, RecordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat), message.ToString()).ConfigureAwait(false);
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
