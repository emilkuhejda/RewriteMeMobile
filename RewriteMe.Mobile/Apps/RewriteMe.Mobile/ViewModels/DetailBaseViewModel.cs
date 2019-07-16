using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Utils;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public abstract class DetailBaseViewModel<T> : ViewModelBase
    {
        private IList<DetailItemViewModel<T>> _detailItems;
        private IEnumerable<ActionBarTileViewModel> _navigationItems;
        private bool _notAvailableData;

        protected DetailBaseViewModel(
            IEmailTask emailTask,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            EmailTask = emailTask;

            CanGoBack = true;

            DeleteCommand = new AsyncCommand(ExecuteDeleteCommandAsync);

            PlayerViewModel = new PlayerViewModel();
        }

        public IEmailTask EmailTask { get; }

        public IAsyncCommand DeleteCommand { get; }

        public IList<DetailItemViewModel<T>> DetailItems
        {
            get => _detailItems;
            set => SetProperty(ref _detailItems, value);
        }

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

        public PlayerViewModel PlayerViewModel { get; }

        private ActionBarTileViewModel SendTileItem { get; set; }

        private ActionBarTileViewModel SaveTileItem { get; set; }

        protected void HandleIsDirtyChanged(object sender, EventArgs e)
        {
            SaveTileItem.IsEnabled = CanExecuteSaveCommand();
        }

        protected IEnumerable<ActionBarTileViewModel> CreateNavigation()
        {
            SendTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Send),
                IsEnabled = CanExecuteSendCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Send-Disabled.svg",
                SelectedCommand = new DelegateCommand(ExecuteSendCommand, CanExecuteSendCommand)
            };

            SaveTileItem = new ActionBarTileViewModel
            {
                Text = Loc.Text(TranslationKeys.Save),
                IsEnabled = CanExecuteSaveCommand(),
                IconKeyEnabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Enabled.svg",
                IconKeyDisabled = "resource://RewriteMe.Mobile.Resources.Images.Save-Disabled.svg",
                SelectedCommand = new AsyncCommand(ExecuteSaveCommandAsync, CanExecuteSaveCommand)
            };

            return new[] { SendTileItem, SaveTileItem };
        }

        private bool CanExecuteSendCommand()
        {
            return ThreadHelper.InvokeOnUiThread(() => EmailTask.CanSendEmail && DetailItems.Any());
        }

        private void ExecuteSendCommand()
        {
            ThreadHelper.InvokeOnUiThread(SendEmailInternal);
        }

        protected abstract void SendEmailInternal();

        protected abstract bool CanExecuteSaveCommand();

        protected abstract Task ExecuteSaveCommandAsync();

        protected abstract Task ExecuteDeleteCommandAsync();

        protected override void DisposeInternal()
        {
            DetailItems?.ForEach(x => x.IsDirtyChanged -= HandleIsDirtyChanged);
            PlayerViewModel?.Dispose();
        }
    }
}
