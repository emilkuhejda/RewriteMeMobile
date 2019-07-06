using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class RecorderOverviewViewModel : ViewModelBase
    {
        private IEnumerable<ActionBarTileViewModel> _navigationItems;

        public RecorderOverviewViewModel(
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            NavigateToRecorderCommand = new AsyncCommand(ExecuteNavigateToRecorderCommandAsync);
        }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        public ICommand NavigateToRecorderCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                await Task.CompletedTask.ConfigureAwait(false);

                NavigationItems = CreateNavigation();
            }
        }

        private IEnumerable<ActionBarTileViewModel> CreateNavigation()
        {
            var audioFilesNavigationItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.AudioFiles),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Overview-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Overview-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteNavigateToOverviewAsync)
            };

            var recordedFilesNavigationItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.RecordedFiles),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.RecorderOverview-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.RecorderOverview-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteNavigateToRecorderOverviewAsync)
            };

            return new[] { audioFilesNavigationItem, recordedFilesNavigationItem };
        }

        private async Task ExecuteNavigateToRecorderCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Recorder).ConfigureAwait(false);
        }

        private async Task ExecuteNavigateToOverviewAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Overview).ConfigureAwait(false);
        }

        private async Task ExecuteNavigateToRecorderOverviewAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.RecorderOverview).ConfigureAwait(false);
        }
    }
}
