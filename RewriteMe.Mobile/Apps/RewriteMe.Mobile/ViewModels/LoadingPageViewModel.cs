using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AppCenter.Push;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class LoadingPageViewModel : ViewModelBase
    {
        private readonly IConnectivityService _connectivityService;
        private readonly IRewriteMeWebService _rewriteMeWebService;

        private string _progressText;

        public LoadingPageViewModel(
            IConnectivityService connectivityService,
            IRewriteMeWebService rewriteMeWebService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _connectivityService = connectivityService;
            _rewriteMeWebService = rewriteMeWebService;

            IsSecurePage = false;
            HasTitleBar = false;
            CanGoBack = false;
            IsDefaultIndicatorVisible = false;
            ProgressText = string.Empty;

            ReloadCommand = new AsyncCommand(ExecuteReloadCommandAsync);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var isPushEnabled = await Push.IsEnabledAsync().ConfigureAwait(false);
                if (!isPushEnabled)
                    await Push.SetEnabledAsync(true).ConfigureAwait(false);

                ProgressText = Loc.Text(TranslationKeys.LoadingData);

                if (!_connectivityService.IsConnected)
                    return;

                var accessToken = navigationParameters.GetValue<B2CAccessToken>();
                if (accessToken != null)
                {
                    var isSuccess = await RegisterUserAsync(accessToken).ConfigureAwait(false);
                    if (isSuccess)
                    {
                        await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}", navigationParameters).ConfigureAwait(false);
                        return;
                    }
                }

                var parameters = new NavigationParameters();
                parameters.Add<UserRegistrationNavigationParameters>(new UserRegistrationNavigationParameters(true));
                await NavigationService.GoBackWithoutAnimationAsync(parameters).ConfigureAwait(false);
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

        private async Task<bool> RegisterUserAsync(B2CAccessToken accessToken)
        {
            ProgressText = Loc.Text(TranslationKeys.UserRegistration);

            var isAlive = await _rewriteMeWebService.IsAliveAsync().ConfigureAwait(false);
            if (!isAlive)
                return false;

            try
            {
                await UserSessionService.RegisterUserAsync(accessToken).ConfigureAwait(false);

                return true;
            }
            catch (ArgumentNullException ex)
            {
                Logger.Error(ExceptionFormatter.FormatException(ex));
            }
            catch (UserRegistrationFailedException ex)
            {
                Logger.Error(ExceptionFormatter.FormatException(ex));
            }

            await UserSessionService.SignOutAsync().ConfigureAwait(false);

            return false;
        }
    }
}
