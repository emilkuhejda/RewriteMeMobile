using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Mobile.Views;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public class BottomNavigationViewModel : BindableBase
    {
        private readonly IInformationMessageService _informationMessageService;
        private readonly INavigationService _navigationService;
        private bool _hasUnopenedMessages;

        public BottomNavigationViewModel(
            ISynchronizationService synchronizationService,
            IInformationMessageService informationMessageService,
            INavigationService navigationService)
        {
            _informationMessageService = informationMessageService;
            _navigationService = navigationService;

            informationMessageService.MessageOpened += HandleMessageOpened;
            synchronizationService.SynchronizationCompleted += HandleSynchronizationCompleted;

            NavigateToOverviewCommand = new AsyncCommand(ExecuteNavigateToOverviewCommandAsync, CanExecuteNavigateToOverviewCommand);
            NavigateToRecorderOverviewCommand = new AsyncCommand(ExecuteNavigateToRecorderOverviewCommandAsync, CanExecuteNavigateToRecorderOverviewCommand);
            NavigateToInformationMessagesCommand = new AsyncCommand(ExecuteNavigateToInformationMessagesCommandAsync, CanExecuteNavigateToInformationMessagesCommand);
            NavigateToSettingsCommand = new AsyncCommand(ExecuteNavigateToSettingsCommandAsync, CanExecuteNavigateToSettingsCommand);

            Page = CurrentPage.Overview;
        }

        private CurrentPage Page { get; set; }

        public bool HasUnopenedMessages
        {
            get => _hasUnopenedMessages;
            set => SetProperty(ref _hasUnopenedMessages, value);
        }

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
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add(NavigationConstants.NavigationBack, true);
            await NavigateToAsync($"/{Pages.Navigation}/{Pages.Overview}", CurrentPage.Overview, navigationParameters).ConfigureAwait(false);
        }

        private bool CanExecuteNavigateToRecorderOverviewCommand()
        {
            return Page != CurrentPage.RecorderOverview;
        }

        private async Task ExecuteNavigateToRecorderOverviewCommandAsync()
        {
            await NavigateToAsync($"/{Pages.Navigation}/{Pages.RecorderOverview}", CurrentPage.RecorderOverview).ConfigureAwait(false);
        }

        private bool CanExecuteNavigateToInformationMessagesCommand()
        {
            return Page != CurrentPage.InformationMessages;
        }

        private async Task ExecuteNavigateToInformationMessagesCommandAsync()
        {
            await NavigateToAsync($"/{Pages.Navigation}/{Pages.InfoOverview}", CurrentPage.InformationMessages).ConfigureAwait(false);
        }

        private bool CanExecuteNavigateToSettingsCommand()
        {
            return Page != CurrentPage.Settings;
        }

        private async Task ExecuteNavigateToSettingsCommandAsync()
        {
            await NavigateToAsync($"/{Pages.Navigation}/{Pages.Settings}", CurrentPage.Settings).ConfigureAwait(false);
        }

        private async Task NavigateToAsync(string name, CurrentPage currentPage, INavigationParameters navigationParameters = null)
        {
            var navigationPage = Application.Current.MainPage as RewriteMeNavigationPage;
            navigationPage?.Pages.ForEach(x => ((ViewModelBase)x.BindingContext).Dispose());

            Page = currentPage;

            await _navigationService.NavigateWithoutAnimationAsync(name, navigationParameters).ConfigureAwait(false);
        }

        private async void HandleMessageOpened(object sender, EventArgs e)
        {
            HasUnopenedMessages = await _informationMessageService.HasUnopenedMessagesForLastWeekAsync().ConfigureAwait(false);
        }

        private async void HandleSynchronizationCompleted(object sender, EventArgs e)
        {
            HasUnopenedMessages = await _informationMessageService.HasUnopenedMessagesForLastWeekAsync().ConfigureAwait(false);
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
