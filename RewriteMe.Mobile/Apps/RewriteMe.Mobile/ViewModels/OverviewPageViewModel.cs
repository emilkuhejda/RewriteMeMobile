using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Events;
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
        private readonly ISchedulerService _schedulerService;

        private string _progressText;
        private ObservableCollection<FileItemViewModel> _fileItems;

        public OverviewPageViewModel(
            IFileItemService fileItemService,
            ISchedulerService schedulerService,
            IInformationMessageService informationMessageService,
            ISynchronizationService synchronizationService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(informationMessageService, synchronizationService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _fileItemService = fileItemService;
            _schedulerService = schedulerService;

            NavigateToCreatePageCommand = new AsyncCommand(ExecuteNavigateToCreatePageCommandAsync);
        }

        public ICommand NavigateToCreatePageCommand { get; }

        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }

        public ObservableCollection<FileItemViewModel> FileItems
        {
            get => _fileItems;
            set => SetProperty(ref _fileItems, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                await InitializeNavigation(CurrentPage.Overview).ConfigureAwait(false);

                _schedulerService.Start();

                ProgressText = Loc.Text(TranslationKeys.ActivityIndicatorCaptionText);

                var isNavigationBack = navigationParameters.GetValue<bool>(NavigationConstants.NavigationBack);
                if (navigationParameters.GetNavigationMode() == NavigationMode.New && !isNavigationBack)
                {
                    var importedFile = navigationParameters.GetValue<ImportedFileNavigationParameters>();
                    if (importedFile != null)
                    {
                        await NavigationService.NavigateWithoutAnimationAsync(Pages.Create, navigationParameters).ConfigureAwait(false);
                    }

                    await SynchronizationAsync().ConfigureAwait(false);
                }

                await InitializeFileItems().ConfigureAwait(false);
                NotAvailableData = !FileItems.Any();
            }
        }

        private async Task SynchronizationAsync()
        {
            ProgressText = Loc.Text(TranslationKeys.LoadingData);

            SynchronizationService.InitializationProgress += OnInitializationProgress;
            await SynchronizationService.InitializeAsync().ConfigureAwait(false);
            SynchronizationService.InitializationProgress -= OnInitializationProgress;

            ProgressText = Loc.Text(TranslationKeys.ActivityIndicatorCaptionText);
        }

        private async Task InitializeFileItems()
        {
            var fileItems = await _fileItemService.GetAllAsync().ConfigureAwait(false);
            FileItems = new ObservableCollection<FileItemViewModel>(fileItems.OrderByDescending(x => x.DateUpdated).Select(x => new FileItemViewModel(x, NavigationService)));
        }

        private void OnInitializationProgress(object sender, ProgressEventArgs e)
        {
            ProgressText = $"{Loc.Text(TranslationKeys.LoadingData)} [{e.PercentageDone}%]";
        }

        protected override async Task RefreshList()
        {
            if (FileItems == null || !FileItems.Any())
                return;

            if (IsCurrent)
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

                var fileItemsToDelete = FileItems.Where(x => !fileItems.Select(file => file.Id).Contains(x.FileItem.Id)).ToList();
                foreach (var fileItemToDelete in fileItemsToDelete)
                {
                    FileItems.Remove(fileItemToDelete);
                }
            }
            else
            {
                await InitializeFileItems().ConfigureAwait(false);
            }
        }

        private async Task ExecuteNavigateToCreatePageCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Create).ConfigureAwait(false);
        }

        protected override async Task ExecuteNavigateToOverviewAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
