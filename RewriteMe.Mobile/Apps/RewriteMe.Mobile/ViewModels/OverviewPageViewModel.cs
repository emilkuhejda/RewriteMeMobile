using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Messages;
using RewriteMe.Domain.WebApi;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Resources.Localization;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class OverviewPageViewModel : OverviewBaseViewModel
    {
        private readonly IFileItemService _fileItemService;
        private readonly IInternalValueService _internalValueService;
        private readonly IFileItemSourceUploader _fileItemSourceUploader;
        private readonly INavigator _navigator;

        private ObservableCollection<FileItemViewModel> _fileItems;

        public OverviewPageViewModel(
            IFileItemService fileItemService,
            IInternalValueService internalValueService,
            IFileItemSourceUploader fileItemSourceUploader,
            INavigator navigator,
            ISynchronizationService synchronizationService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(synchronizationService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _fileItemService = fileItemService;
            _internalValueService = internalValueService;
            _fileItemSourceUploader = fileItemSourceUploader;
            _navigator = navigator;

            fileItemService.UploadProgress += HandleUploadProgress;
            fileItemSourceUploader.StateChanged += HandleStateChanged;

            CreateCommand = new AsyncCommand(ExecuteCreateCommandAsync);
        }

        public ICommand CreateCommand { get; }

        public ObservableCollection<FileItemViewModel> FileItems
        {
            get => _fileItems;
            set
            {
                if (SetProperty(ref _fileItems, value))
                {
                    IsEmptyViewVisible = !value.Any();
                }
            }
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                _navigator.ResetNavigation();

                IndicatorCaption = Loc.Text(TranslationKeys.ActivityIndicatorCaptionText);

                var isApplicationOutOfDate = await _internalValueService.GetValueAsync(InternalValues.IsApplicationOutOfDate).ConfigureAwait(false);
                if (isApplicationOutOfDate)
                {
                    await DialogService.ConfirmAsync(
                        Loc.Text(TranslationKeys.ApplicationIsOutOfDateMessage),
                        okText: Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);
                }

                var isNavigationBack = navigationParameters.GetValue<bool>(NavigationConstants.NavigationBack);
                if (navigationParameters.GetNavigationMode() == NavigationMode.New && !isNavigationBack)
                {
                    await SynchronizationAsync().ConfigureAwait(false);
                }

                await InitializeFileItemsAsync().ConfigureAwait(false);
            }
        }

        protected override async Task RefreshList()
        {
            if (FileItems == null)
                return;

            if (IsLoading)
            {
                await InitializeFileItemsAsync().ConfigureAwait(false);
            }
            else
            {
                var fileItems = (await _fileItemService.GetAllAsync().ConfigureAwait(false)).ToList();
                foreach (var fileItem in fileItems)
                {
                    var viewModel = FileItems.SingleOrDefault(x => x.FileItem.Id == fileItem.Id);
                    if (viewModel != null)
                    {
                        viewModel.Update(fileItem);
                    }
                    else
                    {
                        var fileItemViewModel = FileItems.FirstOrDefault(x => x.FileItem.DateCreated < fileItem.DateCreated);
                        var index = fileItemViewModel == null ? 0 : FileItems.IndexOf(fileItemViewModel);
                        FileItems.Insert(index, new FileItemViewModel(fileItem, NavigationService));
                    }
                }

                RemoveObsoleteItems(fileItems);
                IsEmptyViewVisible = !FileItems.Any();
            }
        }

        private void RemoveObsoleteItems(IList<FileItem> fileItems)
        {
            var deletedFileItems = FileItems.Where(fileItemViewModel => !fileItems.Select(x => x.Id).Contains(fileItemViewModel.FileItem.Id)).ToList();
            if (!deletedFileItems.Any())
                return;

            foreach (var deletedFileItem in deletedFileItems)
            {
                FileItems.Remove(deletedFileItem);
            }

            IsEmptyViewVisible = !FileItems.Any();
        }

        private async Task SynchronizationAsync()
        {
            IndicatorCaption = Loc.Text(TranslationKeys.LoadingData);

            SynchronizationService.InitializationProgress += OnInitializationProgress;
            await SynchronizationService.StartAsync().ConfigureAwait(false);
            SynchronizationService.InitializationProgress -= OnInitializationProgress;

            MessagingCenter.Send(new StartBackgroundServiceMessage(BackgroundServiceType.Synchronizer), nameof(BackgroundServiceType.Synchronizer));

            IndicatorCaption = Loc.Text(TranslationKeys.ActivityIndicatorCaptionText);
        }

        private async Task InitializeFileItemsAsync()
        {
            var fileItems = await _fileItemService.GetAllAsync().ConfigureAwait(false);
            FileItems = new ObservableCollection<FileItemViewModel>(fileItems.OrderByDescending(x => x.DateUpdatedUtc).Select(x => new FileItemViewModel(x, NavigationService)));

            var currentUploadedFile = _fileItemSourceUploader.CurrentUploadedFile;
            if (_fileItemSourceUploader.IsRunning && currentUploadedFile != null)
            {
                UpdateProgress(currentUploadedFile.FileItemId, currentUploadedFile.Progress);
            }
        }

        private async Task ExecuteCreateCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Create).ConfigureAwait(false);
        }

        private void OnInitializationProgress(object sender, ProgressEventArgs e)
        {
            IndicatorCaption = $"{Loc.Text(TranslationKeys.LoadingData)} [{e.PercentageDone}%]";
        }

        private void HandleUploadProgress(object sender, UploadProgressEventArgs e)
        {
            UpdateProgress(e.FileItemId, e.PercentageDone);
        }

        private void UpdateProgress(Guid fileItemId, int progress)
        {
            var fileItem = FileItems.SingleOrDefault(x => x.FileItem.Id == fileItemId);
            if (fileItem == null)
                return;

            fileItem.Progress = progress;
        }

        private async void HandleStateChanged(object sender, ManagerStateChangedEventArgs e)
        {
            var fileItems = await _fileItemService.GetAllAsync().ConfigureAwait(false);
            fileItems.ForEach(fileItem =>
            {
                var fileItemViewModel = FileItems.FirstOrDefault(x => x.FileItem.Id == fileItem.Id);
                if (fileItemViewModel != null)
                {
                    fileItemViewModel.Update(fileItem);
                }
            });
        }

        protected override void DisposeInternal()
        {
            _fileItemService.UploadProgress -= HandleUploadProgress;
            _fileItemSourceUploader.StateChanged -= HandleStateChanged;

            base.DisposeInternal();
        }
    }
}
