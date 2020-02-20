using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Navigation;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;

namespace RewriteMe.Mobile.ViewModels
{
    public class BottomNavigationViewModel : BindableBase
    {
        private readonly IInformationMessageService _informationMessageService;
        private readonly INavigator _navigator;

        private bool _hasUnopenedMessages;

        public BottomNavigationViewModel(
            ISynchronizationService synchronizationService,
            IInformationMessageService informationMessageService,
            INavigator navigator)
        {
            _informationMessageService = informationMessageService;
            _navigator = navigator;

            informationMessageService.MessageOpened += HandleMessageOpened;
            synchronizationService.SynchronizationCompleted += HandleSynchronizationCompleted;

            NavigateToOverviewCommand = new AsyncCommand(ExecuteNavigateToOverviewCommandAsync, CanExecuteNavigateToOverviewCommand);
            NavigateToRecorderOverviewCommand = new AsyncCommand(ExecuteNavigateToRecorderOverviewCommandAsync, CanExecuteNavigateToRecorderOverviewCommand);
            NavigateToInformationMessagesCommand = new AsyncCommand(ExecuteNavigateToInformationMessagesCommandAsync, CanExecuteNavigateToInformationMessagesCommand);
            NavigateToSettingsCommand = new AsyncCommand(ExecuteNavigateToSettingsCommandAsync, CanExecuteNavigateToSettingsCommand);
        }

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
            return _navigator.CurrentPage != RootPage.Overview;
        }

        private async Task ExecuteNavigateToOverviewCommandAsync()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add(NavigationConstants.NavigationBack, true);
            await _navigator.NavigateToAsync($"/{Pages.Navigation}/{Pages.Overview}", RootPage.Overview, navigationParameters).ConfigureAwait(false);
        }

        private bool CanExecuteNavigateToRecorderOverviewCommand()
        {
            return _navigator.CurrentPage != RootPage.RecorderOverview;
        }

        private async Task ExecuteNavigateToRecorderOverviewCommandAsync()
        {
            await _navigator.NavigateToAsync($"/{Pages.Navigation}/{Pages.RecorderOverview}", RootPage.RecorderOverview).ConfigureAwait(false);
        }

        private bool CanExecuteNavigateToInformationMessagesCommand()
        {
            return _navigator.CurrentPage != RootPage.InformationMessages;
        }

        private async Task ExecuteNavigateToInformationMessagesCommandAsync()
        {
            await _navigator.NavigateToAsync($"/{Pages.Navigation}/{Pages.InfoOverview}", RootPage.InformationMessages).ConfigureAwait(false);
        }

        private bool CanExecuteNavigateToSettingsCommand()
        {
            return _navigator.CurrentPage != RootPage.Settings;
        }

        private async Task ExecuteNavigateToSettingsCommandAsync()
        {
            await _navigator.NavigateToAsync($"/{Pages.Navigation}/{Pages.Settings}", RootPage.Settings).ConfigureAwait(false);
        }

        private async void HandleMessageOpened(object sender, EventArgs e)
        {
            HasUnopenedMessages = await _informationMessageService.HasUnopenedMessagesForLastWeekAsync().ConfigureAwait(false);
        }

        private async void HandleSynchronizationCompleted(object sender, EventArgs e)
        {
            HasUnopenedMessages = await _informationMessageService.HasUnopenedMessagesForLastWeekAsync().ConfigureAwait(false);
        }
    }
}
