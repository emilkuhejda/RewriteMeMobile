using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Commands;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Views;
using Xamarin.Forms;

namespace RewriteMe.Mobile.ViewModels
{
    public abstract class ViewModelBase : BindableBase, INavigatedAware, IDisposable
    {
        private string _title;
        private bool _hasTitleBar;
        private bool _canGoBack;

        private bool _disposed;

        protected ViewModelBase(
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
        {
            DialogService = dialogService;
            NavigationService = navigationService;
            Logger = loggerFactory.CreateLogger(GetType());
            OperationScope = new AsyncOperationScope();

            HasTitleBar = true;

            NavigateBackCommand = new AsyncCommand(ExecuteNavigateBackCommandAsync, () => CanGoBack);
            NavigateToSettingsCommand = new AsyncCommand(ExecuteNavigateToSettingsCommandAsync);
        }

        protected IDialogService DialogService { get; }

        protected INavigationService NavigationService { get; }

        protected ILogger Logger { get; }

        public AsyncOperationScope OperationScope { get; }

        public ICommand NavigateBackCommand { get; }

        public ICommand NavigateToSettingsCommand { get; }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool HasTitleBar
        {
            get => _hasTitleBar;
            protected set => SetProperty(ref _hasTitleBar, value);
        }

        public bool CanGoBack
        {
            get => _canGoBack;
            protected set => SetProperty(ref _canGoBack, value);
        }

        protected bool IsCurrent => ((RewriteMeNavigationPage)Application.Current.MainPage).CurrentPage.BindingContext.GetType() == GetType();

        public async void OnNavigatedFrom(INavigationParameters parameters)
        {
            await UnloadDataAsync().ConfigureAwait(false);
        }

        protected virtual async Task UnloadDataAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            await LoadDataAsync(parameters).ConfigureAwait(false);
        }

        protected virtual async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task ExecuteNavigateBackCommandAsync()
        {
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
        }

        private async Task ExecuteNavigateToSettingsCommandAsync()
        {
            await NavigationService.NavigateWithoutAnimationAsync(Pages.Settings).ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                DisposeInternal();
            }

            _disposed = true;
        }

        protected virtual void DisposeInternal() { }
    }
}
