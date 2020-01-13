﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class OverviewPageViewModel : OverviewBaseViewModel
    {
        private readonly IFileItemService _fileItemService;
        private readonly IFileItemSourceUploader _fileItemSourceUploader;

        private ObservableCollection<FileItemViewModel> _fileItems;

        public OverviewPageViewModel(
            IFileItemService fileItemService,
            IFileItemSourceUploader fileItemSourceUploader,
            ISynchronizationService synchronizationService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(synchronizationService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _fileItemService = fileItemService;
            _fileItemSourceUploader = fileItemSourceUploader;

            fileItemSourceUploader.StateChanged += HandleStateChanged;

            CreateCommand = new AsyncCommand(ExecuteCreateCommandAsync);
        }

        public ICommand CreateCommand { get; }

        public ObservableCollection<FileItemViewModel> FileItems
        {
            get => _fileItems;
            set => SetProperty(ref _fileItems, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                IndicatorCaption = Loc.Text(TranslationKeys.ActivityIndicatorCaptionText);

                var isNavigationBack = navigationParameters.GetValue<bool>(NavigationConstants.NavigationBack);
                if (navigationParameters.GetNavigationMode() == NavigationMode.New && !isNavigationBack)
                {
                    await ResetUploadStatusesAsync().ConfigureAwait(false);
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
            }
        }

        private async Task ResetUploadStatusesAsync()
        {
            if (!_fileItemSourceUploader.IsRunning)
            {
                await _fileItemService.ResetUploadStatusesAsync().ConfigureAwait(false);
            }
        }

        private async Task SynchronizationAsync()
        {
            IndicatorCaption = Loc.Text(TranslationKeys.LoadingData);

            SynchronizationService.InitializationProgress += OnInitializationProgress;
            await SynchronizationService.StartAsync().ConfigureAwait(false);
            SynchronizationService.InitializationProgress -= OnInitializationProgress;

            IndicatorCaption = Loc.Text(TranslationKeys.ActivityIndicatorCaptionText);
        }

        private async Task InitializeFileItemsAsync()
        {
            var fileItems = await _fileItemService.GetAllAsync().ConfigureAwait(false);
            FileItems = new ObservableCollection<FileItemViewModel>(fileItems.OrderByDescending(x => x.DateUpdatedUtc).Select(x => new FileItemViewModel(x, NavigationService)));
        }

        private async Task ExecuteCreateCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Create).ConfigureAwait(false);
        }

        private void OnInitializationProgress(object sender, ProgressEventArgs e)
        {
            IndicatorCaption = $"{Loc.Text(TranslationKeys.LoadingData)} [{e.PercentageDone}%]";
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
            _fileItemSourceUploader.StateChanged -= HandleStateChanged;

            base.DisposeInternal();
        }
    }
}
