using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Resources.Localization;
using RewriteMe.Resources.Utils;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecordedDetailPageViewModel : ViewModelBase
    {
        private readonly IRecordedItemService _recordedItemService;
        private readonly IEmailService _emailService;

        private RecordedItem _recordedItem;
        private ActionBarTileViewModel _sendTileItem;
        private ActionBarTileViewModel _saveTileItem;

        private IList<RecordedAudioFileViewModel> _recordedFiles;
        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private bool _notAvailableData;

        public RecordedDetailPageViewModel(
            IRecordedItemService recordedItemService,
            IEmailService emailService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _recordedItemService = recordedItemService;
            _emailService = emailService;

            CanGoBack = true;
            PlayerViewModel = new PlayerViewModel();

            DeleteCommand = new AsyncCommand(ExecuteDeleteCommandAsync);
        }

        public IAsyncCommand DeleteCommand { get; }

        public PlayerViewModel PlayerViewModel { get; }

        public IList<RecordedAudioFileViewModel> RecordedFiles
        {
            get => _recordedFiles;
            private set => SetProperty(ref _recordedFiles, value);
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

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    var recordedItem = navigationParameters.GetValue<RecordedItem>();
                    _recordedItem = await _recordedItemService.GetAsync(recordedItem.Id).ConfigureAwait(false);

                    RecordedFiles?.ForEach(x => x.IsDirtyChanged -= HandleIsDirtyChanged);
                    RecordedFiles = _recordedItem.AudioFiles.OrderBy(x => x.DateCreated).Select(CreateDetailItemViewModel).ToList();

                    NotAvailableData = !RecordedFiles.Any();
                }

                NavigationItems = CreateNavigation();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }

        private RecordedAudioFileViewModel CreateDetailItemViewModel(RecordedAudioFile detailItem)
        {
            RecordedAudioFileViewModel viewModel = new RecordedAudioFileViewModel(PlayerViewModel, DialogService, detailItem);
            viewModel.IsDirtyChanged += HandleIsDirtyChanged;

            return viewModel;
        }

        private IEnumerable<ActionBarTileViewModel> CreateNavigation()
        {
            _sendTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Send),
                IsEnabled = CanExecuteSendCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSendCommandAsync, CanExecuteSendCommand)
            };

            _saveTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Save),
                IsEnabled = CanExecuteSaveCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSaveCommandAsync, CanExecuteSaveCommand)
            };

            return new[] { _sendTileItem, _saveTileItem };
        }

        private bool CanExecuteSaveCommand()
        {
            return RecordedFiles.Any(x => x.IsDirty);
        }

        private async Task ExecuteSaveCommandAsync()
        {
            var recordedAudioFileToSave = RecordedFiles.Where(x => x.IsDirty).Select(x => x.DetailItem);

            await _recordedItemService.UpdateAudioFilesAsync(recordedAudioFileToSave).ConfigureAwait(false);
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        private bool CanExecuteSendCommand()
        {
            return RecordedFiles.Any();
        }

        private async Task ExecuteSendCommandAsync()
        {
            var message = new StringBuilder();
            foreach (var recordedAudioFile in RecordedFiles)
            {
                message.AppendLine(recordedAudioFile.Time);
                message.AppendLine(recordedAudioFile.Transcript);
                message.AppendLine();
            }

            await _emailService.SendAsync(string.Empty, _recordedItem.DateCreated.ToLocalTime().ToString(Constants.TimeFormat), message.ToString()).ConfigureAwait(false);
        }

        private async Task ExecuteDeleteCommandAsync()
        {
            var title = _recordedItem.FileName;
            var result = await DialogService.ConfirmAsync(
                Loc.Text(TranslationKeys.PromptDeleteFileItemMessage, title),
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (result)
            {
                using (new OperationMonitor(OperationScope))
                {
                    await _recordedItemService.DeleteRecordedItemAsync(_recordedItem).ConfigureAwait(false);
                    await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                }
            }
        }

        private void HandleIsDirtyChanged(object sender, EventArgs e)
        {
            _saveTileItem.IsEnabled = CanExecuteSaveCommand();
        }

        protected override void DisposeInternal()
        {
            RecordedFiles?.ForEach(x =>
            {
                x.IsDirtyChanged -= HandleIsDirtyChanged;
            });

            PlayerViewModel?.Dispose();
        }
    }
}
