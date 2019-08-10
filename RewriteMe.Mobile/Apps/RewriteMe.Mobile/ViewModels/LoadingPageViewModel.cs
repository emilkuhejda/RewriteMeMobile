﻿using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Http;
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
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IInternalValueService _internalValueService;
        private readonly ISynchronizationService _synchronizationService;
        private readonly ISchedulerService _schedulerService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IRegistrationUserWebService _registrationUserWebService;

        private string _progressText;

        public LoadingPageViewModel(
            IUserSubscriptionService userSubscriptionService,
            IInternalValueService internalValueService,
            ISynchronizationService synchronizationService,
            ISchedulerService schedulerService,
            IRewriteMeWebService rewriteMeWebService,
            IRegistrationUserWebService registrationUserWebService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _userSubscriptionService = userSubscriptionService;
            _internalValueService = internalValueService;
            _synchronizationService = synchronizationService;
            _schedulerService = schedulerService;
            _rewriteMeWebService = rewriteMeWebService;
            _registrationUserWebService = registrationUserWebService;

            HasTitleBar = false;
            CanGoBack = false;

            ReloadCommand = new AsyncCommand(ExecuteReloadCommandAsync);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var isAlive = await _rewriteMeWebService.IsAliveAsync().ConfigureAwait(false);

                var isUserRegistrationSuccess = await _internalValueService.GetValueAsync(InternalValues.IsUserRegistrationSuccess).ConfigureAwait(false);
                if (!isUserRegistrationSuccess)
                {
                    if (!isAlive)
                        return;

                    ProgressText = Loc.Text(TranslationKeys.UserRegistration);

                    var userSession = await UserSessionService.GetUserSessionAsync().ConfigureAwait(false);
                    var accessToken = await UserSessionService.GetAccessTokenSilentAsync().ConfigureAwait(false);
                    var httpRequestResult = await _registrationUserWebService.RegisterUserAsync(userSession.ToRegisterUserModel(), accessToken).ConfigureAwait(false);
                    if (httpRequestResult.State != HttpRequestState.Success)
                        return;

                    await _userSubscriptionService.AddAsync(httpRequestResult.Payload.UserSubscription).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.IsUserRegistrationSuccess, true).ConfigureAwait(false);
                }

                ProgressText = Loc.Text(TranslationKeys.LoadingData);

                if (isAlive)
                {
                    _synchronizationService.InitializationProgress += OnInitializationProgress;

                    await _synchronizationService.InitializeAsync().ConfigureAwait(false);

                    _synchronizationService.InitializationProgress -= OnInitializationProgress;
                }

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
    }
}
