using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public abstract class OverviewBaseViewModel : ViewModelBase
    {
        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private bool _notAvailableData;
        private bool _isRefreshing;

        protected OverviewBaseViewModel(
            IInformationMessageService informationMessageService,
            ISynchronizationService synchronizationService,
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(userSessionService, dialogService, navigationService, loggerFactory)
        {
            InformationMessageService = informationMessageService;
            SynchronizationService = synchronizationService;
            SynchronizationService.SynchronizationCompleted += HandleSynchronizationCompleted;

            NavigateToRecorderCommand = new AsyncCommand(ExecuteNavigateToRecorderCommandAsync);
            RefreshCommand = new AsyncCommand(ExecuteRefreshCommandAsync);
        }

        protected IInformationMessageService InformationMessageService { get; }

        protected ISynchronizationService SynchronizationService { get; }

        public IEnumerable<ActionBarTileViewModel> NavigationItems
        {
            get => _navigationItems;
            set => SetProperty(ref _navigationItems, value);
        }

        public bool NotAvailableData
        {
            get => _notAvailableData;
            set => SetProperty(ref _notAvailableData, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand NavigateToRecorderCommand { get; }

        public ICommand RefreshCommand { get; }

        private ActionBarTileViewModel InfoNavigationItem { get; set; }

        protected async Task InitializeNavigation(CurrentPage currentPage)
        {
            var audioFilesNavigationItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.AudioFiles),
                IsEnabled = currentPage != CurrentPage.Overview,
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Overview-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Overview-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteNavigateToOverviewAsync)
            };

            var recordedFilesNavigationItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.RecordedFiles),
                IsEnabled = currentPage != CurrentPage.RecorderOverview,
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.RecorderOverview-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.RecorderOverview-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteNavigateToRecorderOverviewAsync)
            };

            var informationMessagesNavigationItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Info),
                IsEnabled = currentPage != CurrentPage.InformationMessages,
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Notification-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Notification-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteNavigateToInformationMessagesAsync)
            };

            NavigationItems = new[] { audioFilesNavigationItem, recordedFilesNavigationItem, informationMessagesNavigationItem };
            InfoNavigationItem = informationMessagesNavigationItem;

            await UpdateNavigationItemIconAsync().ConfigureAwait(false);
        }

        private async Task UpdateNavigationItemIconAsync()
        {
            if (InfoNavigationItem == null)
                return;

            var isUnopenedMessage = await InformationMessageService.IsUnopenedMessageAsync().ConfigureAwait(false);
            InfoNavigationItem.IconKeyEnabled = isUnopenedMessage
                ? "resource://RewriteMe.Mobile.Resources.Images.Notification-Active.svg"
                : "resource://RewriteMe.Mobile.Resources.Images.Notification-Enabled.svg";
            InfoNavigationItem.IconKeyDisabled = isUnopenedMessage
                ? "resource://RewriteMe.Mobile.Resources.Images.Notification-Active-Disabled.svg"
                : "resource://RewriteMe.Mobile.Resources.Images.Notification-Disabled.svg";
        }

        private async Task ExecuteNavigateToRecorderCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Recorder).ConfigureAwait(false);
        }

        private async Task ExecuteRefreshCommandAsync()
        {
            IsRefreshing = true;

            await SynchronizationService.InitializeAsync().ConfigureAwait(false);

            IsRefreshing = false;
        }

        protected abstract Task ExecuteNavigateToOverviewAsync();

        protected abstract Task ExecuteNavigateToRecorderOverviewAsync();

        private async Task ExecuteNavigateToInformationMessagesAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.InfoOverview).ConfigureAwait(false);
        }

        private async void HandleSynchronizationCompleted(object sender, EventArgs e)
        {
            await UpdateNavigationItemIconAsync().ConfigureAwait(false);
        }

        protected override void DisposeInternal()
        {
            SynchronizationService.SynchronizationCompleted -= HandleSynchronizationCompleted;
        }

        protected enum CurrentPage
        {
            Overview = 0,
            RecorderOverview,
            InformationMessages
        }
    }
}
