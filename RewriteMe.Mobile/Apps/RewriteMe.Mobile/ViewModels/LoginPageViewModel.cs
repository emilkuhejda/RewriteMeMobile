using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Configuration;
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
        private readonly ILanguageService _languageService;
        private readonly IApplicationSettings _applicationSettings;

        private string _loginFeedback;
        private bool _isLoading;

        public LoginPageViewModel(
            ILanguageService languageService,
            IApplicationSettings applicationSettings,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _languageService = languageService;
            _applicationSettings = applicationSettings;

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
                await _applicationSettings.InitializeAsync().ConfigureAwait(false);
                await _languageService.InitializeAsync().ConfigureAwait(false);

                var alreadySignedIn = await UserSessionService.IsSignedInAsync().ConfigureAwait(false);
                if (alreadySignedIn)
                {
                    Logger.Info("User is already signed in. Navigate to loading page.");
                    await NavigationService.NavigateWithoutAnimationAsync(Pages.Loading, navigationParameters).ConfigureAwait(false);
                }
                else
                {
                    Logger.Info("No user is currently signed in. Sign in is required.");
                }

                if (navigationParameters.GetNavigationMode() == NavigationMode.Back)
                {
                    var userRegistrationNavigationParameters = navigationParameters.GetValue<UserRegistrationNavigationParameters>();
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
            var accessToken = await UserSessionService.SignUpOrInAsync().ConfigureAwait(false);
            if (accessToken != null)
            {
                LoginFeedback = Loc.Text(TranslationKeys.SignInSuccessful);
                var navigationParameters = new NavigationParameters();
                navigationParameters.Add<B2CAccessToken>(accessToken);
                await NavigationService.NavigateWithoutAnimationAsync(Pages.Loading, navigationParameters).ConfigureAwait(false);
            }
            else
            {
                LoginFeedback = Loc.Text(TranslationKeys.SignInFailed);
            }
        }
    }
}
