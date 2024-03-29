﻿using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class RadialNavigationMenuViewModel
    {
        private readonly INavigationService _navigationService;

        public RadialNavigationMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToCreatePageCommand = new AsyncCommand(ExecuteNavigateToCreatePageCommandAsync);
            NavigateToRecorderCommand = new AsyncCommand(ExecuteNavigateToRecorderCommandAsync);
            NavigateToUserSubscriptionsCommand = new AsyncCommand(ExecuteNavigateToUserSubscriptionsCommandAsync);
        }

        public ICommand NavigateToCreatePageCommand { get; }

        public ICommand NavigateToRecorderCommand { get; }

        public ICommand NavigateToUserSubscriptionsCommand { get; }

        private async Task ExecuteNavigateToCreatePageCommandAsync()
        {
            await _navigationService.NavigateWithoutAnimationAsync(Pages.Create).ConfigureAwait(false);
        }

        private async Task ExecuteNavigateToRecorderCommandAsync()
        {
            await _navigationService.NavigateWithoutAnimationAsync(Pages.Recorder).ConfigureAwait(false);
        }

        private async Task ExecuteNavigateToUserSubscriptionsCommandAsync()
        {
            await _navigationService.NavigateWithoutAnimationAsync(Pages.UserSubscriptions).ConfigureAwait(false);
        }
    }
}
