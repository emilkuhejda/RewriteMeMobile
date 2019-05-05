using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class LoadingPageViewModel : ViewModelBase
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IInternalValueService _internalValueService;
        private readonly ILastUpdatesService _lastUpdatesService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly ISchedulerService _schedulerService;
        private readonly IRegistrationUserWebService _registrationUserWebService;
        private readonly IApplicationSettings _applicationSettings;

        private string _progressText;

        public LoadingPageViewModel(
            IInternalValueService internalValueService,
            IUserSessionService userSessionService,
            ILastUpdatesService lastUpdatesService,
            ISynchronizationService synchronizationService,
            ISchedulerService schedulerService,
            IRegistrationUserWebService registrationUserWebService,
            IApplicationSettings applicationSettings,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _userSessionService = userSessionService;
            _internalValueService = internalValueService;
            _lastUpdatesService = lastUpdatesService;
            _synchronizationService = synchronizationService;
            _schedulerService = schedulerService;
            _registrationUserWebService = registrationUserWebService;
            _applicationSettings = applicationSettings;

            HasTitleBar = false;

            ReloadCommand = new AsyncCommand(ExecuteReloadCommandAsync);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                await _applicationSettings.InitializeAsync().ConfigureAwait(false);

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

                ProgressText = Loc.Text(TranslationKeys.LoadingData);

                _synchronizationService.InitializationProgress += OnInitializationProgress;

                await _lastUpdatesService.InitializeAsync().ConfigureAwait(false);
                if (_lastUpdatesService.IsConnectionSuccessful)
                {
                    await _synchronizationService.InitializeAsync().ConfigureAwait(false);
                }

                _synchronizationService.InitializationProgress -= OnInitializationProgress;

                var isFirstTimeDataSync = await _synchronizationService.IsFirstTimeDataSyncAsync().ConfigureAwait(false);
                if (!isFirstTimeDataSync)
                {
                    _schedulerService.StartAsync();

                    await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}").ConfigureAwait(false);
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
    }
}
