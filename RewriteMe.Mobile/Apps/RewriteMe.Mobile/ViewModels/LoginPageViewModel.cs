using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IConnectivityService _connectivityService;

        private INavigationParameters _navigationParameters;
        private string _loginFeedback;
        private bool _isLoading;

        public LoginPageViewModel(
            IConnectivityService connectivityService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _connectivityService = connectivityService;

            IsSecurePage = false;
            HasTitleBar = false;
            CanGoBack = false;

            LoginCommand = new AsyncCommand(ExecuteLoginCommandAsync, CanExecuteLoginCommand);
        }

        public string LoginFeedback
        {
            get => _loginFeedback;
            set => SetProperty(ref _loginFeedback, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    ThreadHelper.InvokeOnUiThread(() => LoginCommand.ChangeCanExecute());
                }
            }
        }

        public IAsyncCommand LoginCommand { get; }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            IsLoading = true;

            using (new OperationMonitor(OperationScope))
            {
                _navigationParameters = navigationParameters;

                var alreadySignedIn = await UserSessionService.IsSignedInAsync().ConfigureAwait(false);
                if (alreadySignedIn)
                {
                    Logger.Info("User is already signed in. Navigate to loading page.");
                    await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}", _navigationParameters).ConfigureAwait(false);
                }
                else
                {
                    Logger.Info("No user is currently signed in. Sign in is required.");
                }

                if (_navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var userRegistrationNavigationParameters = _navigationParameters.GetValue<UserRegistrationNavigationParameters>();
                    if (userRegistrationNavigationParameters != null && userRegistrationNavigationParameters.IsError)
                    {
                        LoginFeedback = Loc.Text(TranslationKeys.UserRegistrationFailed);
                    }
                }
            }

            IsLoading = false;
        }

        private bool CanExecuteLoginCommand()
        {
            return !_isLoading;
        }

        private async Task ExecuteLoginCommandAsync()
        {
            if (!_connectivityService.IsConnected)
            {
                await DialogService.AlertAsync(Loc.Text(TranslationKeys.OfflineErrorMessage)).ConfigureAwait(false);
                return;
            }

            var accessToken = await UserSessionService.SignUpOrInAsync().ConfigureAwait(false);
            if (accessToken != null)
            {
                LoginFeedback = Loc.Text(TranslationKeys.SignInSuccessful);

                _navigationParameters.Add<B2CAccessToken>(accessToken);
                await NavigationService.NavigateWithoutAnimationAsync(Pages.Loading, _navigationParameters).ConfigureAwait(false);
            }
            else
            {
                LoginFeedback = Loc.Text(TranslationKeys.SignInFailed);
            }
        }
    }
}
