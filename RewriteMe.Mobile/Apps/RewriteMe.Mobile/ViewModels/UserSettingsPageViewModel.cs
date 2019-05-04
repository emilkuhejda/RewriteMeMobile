﻿using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class UserSettingsPageViewModel : ViewModelBase
    {
        private readonly IUserSessionService _userSessionService;

        public UserSettingsPageViewModel(
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _userSessionService = userSessionService;

            CanGoBack = true;

            EditProfileCommand = new AsyncCommand(ExecuteEditProfileCommandAsync);
            LogoutCommand = new AsyncCommand(ExecuteLogoutCommandAsync);
        }

        public ICommand EditProfileCommand { get; }

        public ICommand LogoutCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                Title = await _userSessionService.GetUserNameAsync().ConfigureAwait(false);
            }
        }

        private async Task ExecuteEditProfileCommandAsync()
        {
            await _userSessionService.EditProfileAsync().ConfigureAwait(false);
        }

        private async Task ExecuteLogoutCommandAsync()
        {
            await _userSessionService.SignOutAsync().ConfigureAwait(false);
            await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Login}").ConfigureAwait(false);
        }
    }
}
