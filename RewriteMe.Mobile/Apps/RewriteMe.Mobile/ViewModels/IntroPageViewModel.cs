﻿using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class IntroPageViewModel : ViewModelBase
    {
        private readonly IInternalValueService _internalValueService;

        public IntroPageViewModel(
            IInternalValueService internalValueService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _internalValueService = internalValueService;
            SkipCommand = new AsyncCommand(ExecuteSkipCommandAsync);
            NextCommand = new AsyncCommand(ExecuteNextCommandAsync);
        }

        private INavigationParameters NavigationParameters { get; set; }

        public ICommand SkipCommand { get; }

        public ICommand NextCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            NavigationParameters = navigationParameters;

            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task ExecuteSkipCommandAsync()
        {
            await _internalValueService.UpdateValueAsync(InternalValues.IsIntroSkipped, true).ConfigureAwait(false);
            await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}", NavigationParameters).ConfigureAwait(false);
        }

        private async Task ExecuteNextCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.IntroTwo, NavigationParameters).ConfigureAwait(false);
        }
    }
}
