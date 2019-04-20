using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private readonly IUserSessionService _userSessionService;

        private string _loginFeedback;
        private bool _isLoading;

        public LoginPageViewModel(
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _userSessionService = userSessionService;

            HasTitleBar = false;

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
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoginCommand { get; }

        private bool CanExecuteLoginCommand()
        {
            return true;
        }

        private async Task ExecuteLoginCommandAsync()
        {
            var signinSuccessful = await _userSessionService.SignUpOrInAsync().ConfigureAwait(false);
            if (signinSuccessful)
            {
                LoginFeedback = Loc.Text(TranslationKeys.SignInSuccessful);
                await NavigationService.NavigateWithoutAnimationAsync(Pages.Main).ConfigureAwait(false);
            }
            else
            {
                LoginFeedback = Loc.Text(TranslationKeys.SignInFailed);
            }
        }
    }
}
