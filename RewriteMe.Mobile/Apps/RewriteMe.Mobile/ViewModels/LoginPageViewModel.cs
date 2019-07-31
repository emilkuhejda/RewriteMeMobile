using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IUserSessionService _userSessionService;
        private readonly ILanguageService _languageService;
        private readonly IApplicationSettings _applicationSettings;

        private string _loginFeedback;
        private bool _isLoading;

        public LoginPageViewModel(
            IUserSessionService userSessionService,
            ILanguageService languageService,
            IApplicationSettings applicationSettings,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _userSessionService = userSessionService;
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

                var alreadySignedIn = await _userSessionService.IsSignedInAsync().ConfigureAwait(false);
                if (alreadySignedIn)
                {
                    Logger.Info("User is already signed in. Navigate to loading page.");
                    await NavigationService.NavigateWithoutAnimationAsync(Pages.Loading, navigationParameters).ConfigureAwait(false);
                }
                else
                {
                    Logger.Info("No user is currently signed in. Sign in is required.");
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
            var signinSuccessful = await _userSessionService.SignUpOrInAsync().ConfigureAwait(false);
            if (signinSuccessful)
            {
                LoginFeedback = Loc.Text(TranslationKeys.SignInSuccessful);
                await NavigationService.NavigateWithoutAnimationAsync(Pages.Loading).ConfigureAwait(false);
            }
            else
            {
                LoginFeedback = Loc.Text(TranslationKeys.SignInFailed);
            }
        }
    }
}
