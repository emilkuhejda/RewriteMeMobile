using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Events;
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
        private readonly ISynchronizationService _synchronizationService;
        private readonly ISchedulerService _schedulerService;

        private string _progressText;

        public LoadingPageViewModel(
            ISynchronizationService synchronizationService,
            ISchedulerService schedulerService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _synchronizationService = synchronizationService;
            _schedulerService = schedulerService;

            HasTitleBar = false;
            CanGoBack = false;

            ReloadCommand = new AsyncCommand(ExecuteReloadCommandAsync);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var accessToken = navigationParameters.GetValue<B2CAccessToken>();
                if (accessToken != null)
                {
                    var isSuccess = await RegisterUserAsync(accessToken).ConfigureAwait(false);
                    if (!isSuccess)
                    {
                        var parameters = new NavigationParameters();
                        parameters.Add<UserRegistrationNavigationParameters>(new UserRegistrationNavigationParameters(true));
                        await NavigationService.GoBackWithoutAnimationAsync(parameters).ConfigureAwait(false);
                        return;
                    }
                }

                ProgressText = Loc.Text(TranslationKeys.LoadingData);

                _synchronizationService.InitializationProgress += OnInitializationProgress;
                await _synchronizationService.InitializeAsync().ConfigureAwait(false);
                _synchronizationService.InitializationProgress -= OnInitializationProgress;

                var isFirstTimeDataSync = await _synchronizationService.IsFirstTimeDataSyncAsync().ConfigureAwait(false);
                if (!isFirstTimeDataSync)
                {
                    _schedulerService.StartAsync();

                    await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}", navigationParameters).ConfigureAwait(false);
                }
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

        private void OnInitializationProgress(object sender, ProgressEventArgs e)
        {
            ProgressText = $"{Loc.Text(TranslationKeys.LoadingData)} [{e.PercentageDone}%]";
        }

        private async Task<bool> RegisterUserAsync(B2CAccessToken accessToken)
        {
            ProgressText = Loc.Text(TranslationKeys.UserRegistration);

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
