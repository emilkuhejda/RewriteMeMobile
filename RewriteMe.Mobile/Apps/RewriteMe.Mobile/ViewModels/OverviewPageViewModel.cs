using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.LatestVersion.Abstractions;
using Plugin.Messaging;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;

namespace RewriteMe.Mobile.ViewModels
{
    public class OverviewPageViewModel : OverviewBaseViewModel
    {
        private readonly IFileItemService _fileItemService;
        private readonly ISchedulerService _schedulerService;

        private IList<FileItemViewModel> _fileItems;

        public OverviewPageViewModel(
            IFileItemService fileItemService,
            ISchedulerService schedulerService,
            IUserSessionService userSessionService,
            IInternalValueService internalValueService,
            ILatestVersion latestVersion,
            IEmailTask emailTask,
            IApplicationSettings applicationSettings,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, internalValueService, latestVersion, emailTask, applicationSettings, dialogService, navigationService, loggerFactory)
        {
            _fileItemService = fileItemService;
            _schedulerService = schedulerService;

            _schedulerService.SynchronizationCompleted += HandleSynchronizationCompleted;

            NavigateToCreatePageCommand = new AsyncCommand(ExecuteNavigateToCreatePageCommandAsync);
        }

        public ICommand NavigateToCreatePageCommand { get; }

        public IList<FileItemViewModel> FileItems
        {
            get => _fileItems;
            set => SetProperty(ref _fileItems, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                if (navigationParameters.GetNavigationMode() == NavigationMode.New)
                {
                    var importedFile = navigationParameters.GetValue<ImportedFileNavigationParameters>();
                    if (importedFile != null)
                    {
                        await NavigationService.NavigateWithoutAnimationAsync(Pages.Create, navigationParameters).ConfigureAwait(false);
                    }
                }

                await InitializeFileItems().ConfigureAwait(false);

                IsNotUserRegistrationSuccess = !await InternalValueService.GetValueAsync(InternalValues.IsUserRegistrationSuccess).ConfigureAwait(false);
                NotAvailableData = !FileItems.Any();

                InitializeNavigation(true);
            }
        }

        private async Task InitializeFileItems()
        {
            var fileItems = await _fileItemService.GetAllAsync().ConfigureAwait(false);
            FileItems = fileItems.OrderByDescending(x => x.DateUpdated).Select(x => new FileItemViewModel(x, NavigationService)).ToList();
        }

        private async Task ExecuteNavigateToCreatePageCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Create).ConfigureAwait(false);
        }

        private async void HandleSynchronizationCompleted(object sender, EventArgs e)
        {
            if (!FileItems.Any())
                return;

            if (IsCurrent)
            {
                var fileItems = await _fileItemService.GetAllAsync().ConfigureAwait(false);
                foreach (var fileItem in fileItems)
                {
                    var viewModel = FileItems.SingleOrDefault(x => x.FileItem.Id == fileItem.Id && x.FileItem.RecognitionState != fileItem.RecognitionState);
                    viewModel?.Update(fileItem);
                }
            }
            else
            {
                await InitializeFileItems().ConfigureAwait(false);
            }
        }

        protected override async Task ExecuteNavigateToOverviewAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        protected override async Task ExecuteNavigateToRecorderOverviewAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.RecorderOverview).ConfigureAwait(false);
        }

        protected override void DisposeInternal()
        {
            _schedulerService.SynchronizationCompleted -= HandleSynchronizationCompleted;
        }
    }
}
