using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class UserSettingsPageViewModel : ViewModelBase
    {
        private readonly IRewriteMeWebService _rewriteMeWebService;

        public UserSettingsPageViewModel(
            IRewriteMeWebService rewriteMeWebService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _rewriteMeWebService = rewriteMeWebService;

            CanGoBack = true;

            ResetPasswordCommand = new AsyncCommand(ExecuteResetPasswordCommandAsync);
            DeleteAccountCommand = new AsyncCommand(ExecuteDeleteAccountCommandAsync);
            LogoutCommand = new AsyncCommand(ExecuteLogoutCommandAsync);
        }

        public ICommand ResetPasswordCommand { get; }

        public ICommand DeleteAccountCommand { get; }

        public ICommand LogoutCommand { get; }

        private async Task ExecuteResetPasswordCommandAsync()
        {
            var result = await UserSessionService.ResetPasswordAsync().ConfigureAwait(false);
            if (result != null)
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.ProfileEditErrorMessage)).ConfigureAwait(false);
            }
        }

        private async Task ExecuteDeleteAccountCommandAsync()
        {
            var userSession = await UserSessionService.GetUserSessionAsync().ConfigureAwait(false);
            var message = Loc.Text(TranslationKeys.DeleteUserAccountMessage, userSession.Email);
            var result = await DialogService.PromptAsync(
                message,
                okText: Loc.Text(TranslationKeys.Ok),
                cancelText: Loc.Text(TranslationKeys.Cancel)).ConfigureAwait(false);

            if (!result.Ok)
                return;

            if (!result.Text.Equals(userSession.Email, StringComparison.OrdinalIgnoreCase))
            {
                await DialogService.AlertAsync(
                    Loc.Text(TranslationKeys.IncorrectUserInputErrorMessage),
                    Loc.Text(TranslationKeys.Warning),
                    Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);
                return;
            }

            using (new OperationMonitor(OperationScope))
            {
                var httpRequestResult = await _rewriteMeWebService.DeleteUserAsync().ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    await ExecuteLogoutCommandAsync().ConfigureAwait(false);
                    return;
                }
            }

            await DialogService.AlertAsync(
                Loc.Text(TranslationKeys.UserAccountDeletionErrorMessage),
                Loc.Text(TranslationKeys.Warning),
                Loc.Text(TranslationKeys.Ok)).ConfigureAwait(false);
        }

        private async Task ExecuteLogoutCommandAsync()
        {
            await UserSessionService.SignOutAsync().ConfigureAwait(false);
            await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Login}").ConfigureAwait(false);
        }
    }
}
