using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class DetailPageViewModel : ViewModelBase
    {
        private readonly ITranscribeItemService _transcribeItemService;
        private readonly ITranscriptAudioSourceService _transcriptAudioSourceService;
        private readonly IFileItemService _fileItemService;
        private readonly ITranscribeItemManager _transcribeItemManager;
        private readonly IEmailService _emailService;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private IList<DetailItemViewModel<TranscribeItem>> _transcribeItems;
        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private bool _isPopupOpen;
        private double _progress;
        private string _progressText;
        private bool _notAvailableData;

        public DetailPageViewModel(
            ITranscribeItemService transcribeItemService,
            ITranscriptAudioSourceService transcriptAudioSourceService,
            IFileItemService fileItemService,
            ITranscribeItemManager transcribeItemManager,
            IInternalValueService internalValueService,
            IEmailService emailService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _transcribeItemService = transcribeItemService;
            _transcriptAudioSourceService = transcriptAudioSourceService;
            _fileItemService = fileItemService;
            _transcribeItemManager = transcribeItemManager;
            _emailService = emailService;

            CanGoBack = true;
            SettingsViewModel = new SettingsViewModel(internalValueService);
            PlayerViewModel = new PlayerViewModel();

            DeleteCommand = new AsyncCommand(ExecuteDeleteCommandAsync);
            OpenSettingsCommand = new DelegateCommand(ExecuteOpenSettingsCommand);

            _transcribeItemManager.StateChanged += HandleStateChanged;
            _transcribeItemManager.InitializationProgress += HandleInitializationProgress;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public IAsyncCommand DeleteCommand { get; }

        public ICommand OpenSettingsCommand { get; }

        private FileItem FileItem { get; set; }

        public SettingsViewModel SettingsViewModel { get; set; }

        public PlayerViewModel PlayerViewModel { get; }

        private ActionBarTileViewModel SendTileItem { get; set; }

        private ActionBarTileViewModel SaveTileItem { get; set; }

        public bool IsProgressVisible => _transcribeItemManager.IsRunning;

        public IList<DetailItemViewModel<TranscribeItem>> TranscribeItems
        {
            get => _transcribeItems;
            private set => SetProperty(ref _transcribeItems, value);
        }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value);
        }

        public double Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
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
                await SettingsViewModel.InitializeAsync().ConfigureAwait(false);
                InitializeProgressLabel();

                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    FileItem = navigationParameters.GetValue<FileItem>();

                    var transcribeItems = await _transcribeItemService.GetAllAsync(FileItem.Id).ConfigureAwait(false);

                    TranscribeItems?.ForEach(x => x.IsDirtyChanged -= HandleIsDirtyChanged);
                    TranscribeItems = transcribeItems.OrderBy(x => x.StartTime).Select(CreateDetailItemViewModel).ToList();

                    NotAvailableData = !TranscribeItems.Any();
                }

                NavigationItems = CreateNavigation();
            }
        }

        private void InitializeProgressLabel()
        {
            ProgressText = Loc.Text(TranslationKeys.Downloading, 0);
        }

        private DetailItemViewModel<TranscribeItem> CreateDetailItemViewModel(TranscribeItem detailItem)
        {
            var viewModel = new TranscribeItemViewModel(
                _transcriptAudioSourceService,
                _transcribeItemManager,
                SettingsViewModel,
                PlayerViewModel,
                DialogService,
                detailItem,
                _cancellationTokenSource.Token);
            viewModel.IsDirtyChanged += HandleIsDirtyChanged;

            return viewModel;
        }

        private IEnumerable<ActionBarTileViewModel> CreateNavigation()
        {
            SendTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Send),
                IsEnabled = CanExecuteSendCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSendCommandAsync, CanExecuteSendCommand)
            };

            SaveTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Save),
                IsEnabled = CanExecuteSaveCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSaveCommandAsync, CanExecuteSaveCommand)
            };

            return new[] { SendTileItem, SaveTileItem };
        }

        private bool CanExecuteSendCommand()
        {
            return TranscribeItems.Any();
        }

        private async Task ExecuteSendCommandAsync()
        {
            var message = new StringBuilder();
            foreach (var transcribeItem in TranscribeItems)
            {
                message.AppendLine($"{transcribeItem.Time} {transcribeItem.Accuracy}");
                message.AppendLine(transcribeItem.Transcript);
                message.AppendLine().AppendLine();
            }

            await _emailService.SendAsync(string.Empty, FileItem.Name, message.ToString()).ConfigureAwait(false);
        }

        private bool CanExecuteSaveCommand()
        {
            return TranscribeItems.Any(x => x.IsDirty);
        }

        private async Task ExecuteSaveCommandAsync()
        {
            var transcribeItemsToSave = TranscribeItems.Where(x => x.IsDirty).Select(x => x.DetailItem);

            await _transcribeItemService.SaveAndSendAsync(transcribeItemsToSave).ConfigureAwait(false);
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        private async Task ExecuteDeleteCommandAsync()
        {
            var result = await DialogService.ConfirmAsync(
                Loc.Text(TranslationKeys.PromptDeleteFileItemMessage, FileItem.Name),
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (result)
            {
                using (new OperationMonitor(OperationScope))
                {
                    try
                    {
                        await _fileItemService.DeleteAsync(FileItem).ConfigureAwait(false);
                        await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
                    }
                    catch (FileNotUploadedException)
                    {
                        await DialogService.AlertAsync(Loc.Text(TranslationKeys.FileIsNotUploadedErrorMessage)).ConfigureAwait(false);
                    }
                }
            }
        }

        private void ExecuteOpenSettingsCommand()
        {
            IsPopupOpen = !IsPopupOpen;
        }

        private void HandleIsDirtyChanged(object sender, EventArgs e)
        {
            SaveTileItem.IsEnabled = CanExecuteSaveCommand();
        }

        private void HandleStateChanged(object sender, ManagerStateChangedEventArgs e)
        {
            InitializeProgressLabel();
            RaisePropertyChanged(nameof(IsProgressVisible));
        }

        private void HandleInitializationProgress(object sender, ProgressEventArgs e)
        {
            Progress = e.PercentageDone / 100d;
            ProgressText = Loc.Text(TranslationKeys.Downloading, e.PercentageDone);
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();

            _transcribeItemManager.StateChanged -= HandleStateChanged;
            _transcribeItemManager.InitializationProgress -= HandleInitializationProgress;
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            TranscribeItems?.ForEach(x =>
            {
                x.IsDirtyChanged -= HandleIsDirtyChanged;
                x.Dispose();
            });

            PlayerViewModel?.Dispose();
        }
    }
}
