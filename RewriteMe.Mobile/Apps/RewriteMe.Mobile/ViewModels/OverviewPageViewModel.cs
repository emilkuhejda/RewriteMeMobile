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

        private ObservableCollection<FileItemViewModel> _fileItems;

        public OverviewPageViewModel(
            IFileItemService fileItemService,
            ISynchronizationService synchronizationService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(synchronizationService, userSessionService, dialogService, navigationService, loggerFactory)
        {
            _fileItemService = fileItemService;

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
                    var importedFile = navigationParameters.GetValue<ImportedFileNavigationParameters>();
                    if (importedFile != null)
                    {
                        await NavigationService.NavigateWithoutAnimationAsync(Pages.Create, navigationParameters).ConfigureAwait(false);
                    }

                    await SynchronizationAsync().ConfigureAwait(false);
                }

                await InitializeFileItemsAsync().ConfigureAwait(false);
            }
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
                await InitializeFileItemsAsync().ConfigureAwait(false);
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
    }
}
