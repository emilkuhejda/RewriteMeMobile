using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
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
            ResetPasswordCommand = new AsyncCommand(ExecuteResetPasswordCommandAsync);
            LogoutCommand = new AsyncCommand(ExecuteLogoutCommandAsync);
        }

        public ICommand EditProfileCommand { get; }

        public ICommand ResetPasswordCommand { get; }

        public ICommand LogoutCommand { get; }

        private async Task ExecuteEditProfileCommandAsync()
        {
            await _userSessionService.EditProfileAsync().ConfigureAwait(false);
        }

        private async Task ExecuteResetPasswordCommandAsync()
        {
            await _userSessionService.ResetPasswordAsync().ConfigureAwait(false);
        }

        private async Task ExecuteLogoutCommandAsync()
        {
            await _userSessionService.SignOutAsync().ConfigureAwait(false);
            await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Login}").ConfigureAwait(false);
        }
    }
}
