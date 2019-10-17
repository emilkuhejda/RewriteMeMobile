using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Navigation;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;

namespace RewriteMe.Mobile.ViewModels
{
    public class BottomNavigationViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;

        public BottomNavigationViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToOverviewCommand = new AsyncCommand(ExecuteNavigateToOverviewCommandAsync, CanExecuteNavigateToOverviewCommand);
            NavigateToRecorderOverviewCommand = new AsyncCommand(ExecuteNavigateToRecorderOverviewCommandAsync, CanExecuteNavigateToRecorderOverviewCommand);
            NavigateToInformationMessagesCommand = new AsyncCommand(ExecuteNavigateToInformationMessagesCommandAsync, CanExecuteNavigateToInformationMessagesCommand);
            NavigateToSettingsCommand = new AsyncCommand(ExecuteNavigateToSettingsCommandAsync, CanExecuteNavigateToSettingsCommand);

            Page = CurrentPage.Overview;
        }

        private CurrentPage Page { get; set; }

        public ICommand NavigateToOverviewCommand { get; }

        public ICommand NavigateToRecorderOverviewCommand { get; }

        public ICommand NavigateToInformationMessagesCommand { get; }

        public ICommand NavigateToSettingsCommand { get; }

        private bool CanExecuteNavigateToOverviewCommand()
        {
            return Page != CurrentPage.Overview;
        }

        private async Task ExecuteNavigateToOverviewCommandAsync()
        {
            Page = CurrentPage.Overview;

            var navigationParameters = new NavigationParameters();
            navigationParameters.Add(NavigationConstants.NavigationBack, true);
            await _navigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}", navigationParameters).ConfigureAwait(false);
        }

        private bool CanExecuteNavigateToRecorderOverviewCommand()
        {
            return Page != CurrentPage.RecorderOverview;
        }

        private async Task ExecuteNavigateToRecorderOverviewCommandAsync()
        {
            Page = CurrentPage.RecorderOverview;

            await _navigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.RecorderOverview}").ConfigureAwait(false);
        }

        private bool CanExecuteNavigateToInformationMessagesCommand()
        {
            return Page != CurrentPage.InformationMessages;
        }

        private async Task ExecuteNavigateToInformationMessagesCommandAsync()
        {
            Page = CurrentPage.InformationMessages;

            await _navigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.InfoOverview}").ConfigureAwait(false);
        }

        private bool CanExecuteNavigateToSettingsCommand()
        {
            return Page != CurrentPage.Settings;
        }

        private async Task ExecuteNavigateToSettingsCommandAsync()
        {
            Page = CurrentPage.Settings;

            await _navigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Settings}").ConfigureAwait(false);
        }

        private enum CurrentPage
        {
            Overview = 0,
            RecorderOverview,
            InformationMessages,
            Settings
        }
    }
}
