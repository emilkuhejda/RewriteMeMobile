using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Events;
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
        private readonly ISynchronizationService _synchronizationService;
        private readonly ISchedulerService _schedulerService;
        private readonly IRewriteMeWebService _rewriteMeWebService;

        private string _progressText;

        public LoadingPageViewModel(
            ISynchronizationService synchronizationService,
            ISchedulerService schedulerService,
            IRewriteMeWebService rewriteMeWebService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            _synchronizationService = synchronizationService;
            _schedulerService = schedulerService;
            _rewriteMeWebService = rewriteMeWebService;

            HasTitleBar = false;
            CanGoBack = false;

            ReloadCommand = new AsyncCommand(ExecuteReloadCommandAsync);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var isAlive = await _rewriteMeWebService.IsAliveAsync().ConfigureAwait(false);

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
