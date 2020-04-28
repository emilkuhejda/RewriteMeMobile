﻿using System.Linq;
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
using RewriteMe.Mobile.Extensions;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class DetailPageViewModel : DetailBaseViewModel<TranscribeItem>
    {
        private readonly ITranscribeItemService _transcribeItemService;
        private readonly ITranscriptAudioSourceService _transcriptAudioSourceService;
        private readonly IFileItemService _fileItemService;
        private readonly ITranscribeItemManager _transcribeItemManager;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private bool _isPopupOpen;
        private double _progress;
        private string _progressText;

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
            : base(emailService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _transcribeItemService = transcribeItemService;
            _transcriptAudioSourceService = transcriptAudioSourceService;
            _fileItemService = fileItemService;
            _transcribeItemManager = transcribeItemManager;

            _transcribeItemManager.StateChanged += HandleStateChanged;
            _transcribeItemManager.InitializationProgress += HandleInitializationProgress;

            _cancellationTokenSource = new CancellationTokenSource();

            Settings = new DetailPageSettingsViewModel(internalValueService);
            OpenSettingsCommand = new DelegateCommand(ExecuteOpenSettingsCommand);
        }

        public ICommand OpenSettingsCommand { get; }

        private FileItem FileItem { get; set; }

        public DetailPageSettingsViewModel Settings { get; set; }

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

        public bool IsProgressVisible => _transcribeItemManager.IsRunning;

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                await Settings.InitializeAsync().ConfigureAwait(false);
                InitializeProgressLabel();

                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    FileItem = navigationParameters.GetValue<FileItem>();

                    var transcribeItems = await _transcribeItemService.GetAllAsync(FileItem.Id).ConfigureAwait(false);

                    DetailItems?.ForEach(x => x.IsDirtyChanged -= HandleIsDirtyChanged);
                    DetailItems = transcribeItems.OrderBy(x => x.StartTime).Select(CreateDetailItemViewModel).ToList();

                    NotAvailableData = !DetailItems.Any();
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
                PlayerViewModel,
                DialogService,
                detailItem,
                _cancellationTokenSource.Token);
            viewModel.IsDirtyChanged += HandleIsDirtyChanged;

            return viewModel;
        }

        protected override async Task SendEmailInternal()
        {
            var message = new StringBuilder();
            foreach (var transcribeItem in DetailItems)
            {
                message.AppendLine($"{transcribeItem.Time} {transcribeItem.Accuracy}");
                message.AppendLine(transcribeItem.Transcript);
                message.AppendLine().AppendLine();
            }

            await EmailService.SendAsync(string.Empty, FileItem.Name, message.ToString()).ConfigureAwait(false);
        }

        private void ExecuteOpenSettingsCommand()
        {
            IsPopupOpen = !IsPopupOpen;
        }

        protected override bool CanExecuteSaveCommand()
        {
            return DetailItems.Any(x => x.IsDirty);
        }

        protected override async Task ExecuteSaveCommandAsync()
        {
            var transcribeItemsToSave = DetailItems.Where(x => x.IsDirty).Select(x => x.DetailItem);

            await _transcribeItemService.SaveAndSendAsync(transcribeItemsToSave).ConfigureAwait(false);
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        protected override async Task ExecuteDeleteCommandAsync()
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
        }
    }
}
