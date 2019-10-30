using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;

namespace RewriteMe.Mobile.ViewModels
{
    public abstract class OverviewBaseViewModel : ViewModelBase
    {
        private bool _isRefreshing;

        protected OverviewBaseViewModel(
            ISynchronizationService synchronizationService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            SynchronizationService = synchronizationService;
            SynchronizationService.SynchronizationCompleted += HandleSynchronizationCompleted;

            HasBottomNavigation = true;

            NavigationMenu = new RadialNavigationMenuViewModel(navigationService);

            RefreshCommand = new AsyncCommand(ExecuteRefreshCommandAsync);
        }

        protected ISynchronizationService SynchronizationService { get; }

        public RadialNavigationMenuViewModel NavigationMenu { get; }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand RefreshCommand { get; }

        private async Task ExecuteRefreshCommandAsync()
        {
            IsRefreshing = true;

            await HandleWebServiceCallAsync(async () =>
            {
                await SynchronizationService.StartAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);

            IsRefreshing = false;
        }

        private async void HandleSynchronizationCompleted(object sender, EventArgs e)
        {
            await RefreshList().ConfigureAwait(false);
        }

        protected virtual async Task RefreshList()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        protected override void DisposeInternal()
        {
            SynchronizationService.SynchronizationCompleted -= HandleSynchronizationCompleted;
        }
    }
}
