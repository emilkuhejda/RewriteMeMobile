using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class LoadingPageViewModel : ViewModelBase
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IInternalValueService _internalValueService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IRegistrationUserWebService _registrationUserWebService;

        private string _progressText;

        public LoadingPageViewModel(
            IInternalValueService internalValueService,
            IUserSessionService userSessionService,
            IRewriteMeWebService rewriteMeWebService,
            IRegistrationUserWebService registrationUserWebService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _userSessionService = userSessionService;
            _internalValueService = internalValueService;
            _rewriteMeWebService = rewriteMeWebService;
            _registrationUserWebService = registrationUserWebService;

            HasTitleBar = false;

            ReloadCommand = new AsyncCommand(ExecuteReloadCommandAsync);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var isUserRegistrationSuccess = await _internalValueService.GetValueAsync(InternalValues.IsUserRegistrationSuccess).ConfigureAwait(false);
                if (!isUserRegistrationSuccess)
                {
                    ProgressText = Loc.Text(TranslationKeys.UserRegistration);

                    var userSession = await _userSessionService.GetUserSessionAsync().ConfigureAwait(false);
                    var httpRequestResult = await _registrationUserWebService.RegisterUserAsync(userSession.ToRegisterUserModel()).ConfigureAwait(false);
                    if (httpRequestResult.State != HttpRequestState.Success)
                        return;

                    await _internalValueService.UpdateValueAsync(InternalValues.IsUserRegistrationSuccess, true);
                }

                //await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}").ConfigureAwait(false);
            }
        }

        public ICommand ReloadCommand { get; }

        public string ProgressText
        {
            get => _progressText;
            set => SetProperty(ref _progressText, value);
        }

        private async Task ExecuteReloadCommandAsync()
        {
            await LoadDataAsync(null).ConfigureAwait(false);
        }
    }
}
