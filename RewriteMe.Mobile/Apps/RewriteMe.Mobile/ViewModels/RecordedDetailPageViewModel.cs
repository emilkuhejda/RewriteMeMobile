using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;
using RewriteMe.Resources.Utils;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedDetailPageViewModel : ViewModelBase
    {
        private readonly IRecordedItemService _recordedItemService;
        private readonly IMediaService _mediaService;
        private readonly IEmailTask _emailTask;

        private IList<RecordedAudioFileViewModel> _recordedAudioFiles;
        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private bool _notAvailableData;

        public RecordedDetailPageViewModel(
            IRecordedItemService recordedItemService,
            IMediaService mediaService,
            IEmailTask emailTask,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
            _mediaService = mediaService;
            _emailTask = emailTask;

            CanGoBack = true;

            DeleteCommand = new AsyncCommand(ExecuteDeleteCommandAsync);

            PlayerViewModel = new PlayerViewModel();
        }

        private RecordedItem RecordedItem { get; set; }

        public IList<RecordedAudioFileViewModel> RecordedAudioFiles
        {
            get => _recordedAudioFiles;
            set => SetProperty(ref _recordedAudioFiles, value);
        }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        public bool NotAvailableData
        {
            get => _notAvailableData;
            set => SetProperty(ref _notAvailableData, value);
        }

        public PlayerViewModel PlayerViewModel { get; }

        private ActionBarTileViewModel SendTileItem { get; set; }

        private ActionBarTileViewModel SaveTileItem { get; set; }

        public IAsyncCommand DeleteCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    RecordedItem = navigationParameters.GetValue<RecordedItem>();

                    RecordedAudioFiles?.ForEach(x => x.IsDirtyChanged -= HandleIsDirtyChanged);
                    RecordedAudioFiles = RecordedItem.AudioFiles.OrderBy(x => x.DateCreated).Select(CreateRecordedAudioFileViewModel).ToList();

                    NotAvailableData = !RecordedAudioFiles.Any();
                }

                InitializeNavigation();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private RecordedAudioFileViewModel CreateRecordedAudioFileViewModel(RecordedAudioFile recordedAudioFile)
        {
            var viewModel = new RecordedAudioFileViewModel(PlayerViewModel, recordedAudioFile);
            viewModel.IsDirtyChanged += HandleIsDirtyChanged;

            return viewModel;
        }

        private void HandleIsDirtyChanged(object sender, EventArgs e)
        {
            SaveTileItem.IsEnabled = CanExecuteSaveCommand();
        }

        private void InitializeNavigation()
        {
            SendTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Send),
                IsEnabled = CanExecuteSendCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Disabled.svg",
                SelectedCommand = new DelegateCommand(ExecuteSendCommand, CanExecuteSendCommand)
            };

            SaveTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Save),
                IsEnabled = CanExecuteSaveCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSaveCommandAsync, CanExecuteSaveCommand)
            };

            NavigationItems = new[] { SendTileItem, SaveTileItem };
        }

        private bool CanExecuteSendCommand()
        {
            return _emailTask.CanSendEmail && RecordedAudioFiles.Any();
        }

        private void ExecuteSendCommand()
        {
            ThreadHelper.InvokeOnUiThread(SendEmailInternal);
        }

        private void SendEmailInternal()
        {
            var message = new StringBuilder();
            foreach (var recordedAudioFile in RecordedAudioFiles)
            {
                message.AppendLine(recordedAudioFile.Transcript);
                message.AppendLine();
            }

            _emailTask.SendEmail(
                subject: RecordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat),
                message: message.ToString());
        }

        private bool CanExecuteSaveCommand()
        {
            return RecordedAudioFiles.Any(x => x.IsDirty);
        }

        private async Task ExecuteSaveCommandAsync()
        {
            var recordedAudioFileToSave = RecordedAudioFiles.Where(x => x.IsDirty).Select(x => x.RecordedAudioFile);

            await _recordedItemService.UpdateAudioFilesAsync(recordedAudioFileToSave).ConfigureAwait(false);
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        private async Task ExecuteDeleteCommandAsync()
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

        protected override void DisposeInternal()
        {
            RecordedAudioFiles?.ForEach(x => x.IsDirtyChanged -= HandleIsDirtyChanged);
            PlayerViewModel?.Dispose();
        }
    }
}
