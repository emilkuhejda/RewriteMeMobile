using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Services;
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
        private bool _hasTitleBar;
        private bool _canGoBack;

        private bool _disposed;

        protected ViewModelBase(
            IUserSessionService userSessionService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
        {
            UserSessionService = userSessionService;
            DialogService = dialogService;
            NavigationService = navigationService;
            Logger = loggerFactory.CreateLogger(GetType());
            OperationScope = new AsyncOperationScope();

            IsSecurePage = true;
            HasTitleBar = true;

            NavigateBackCommand = new AsyncCommand(ExecuteNavigateBackCommandAsync, () => CanGoBack);
        }

        protected IUserSessionService UserSessionService { get; }

        protected IDialogService DialogService { get; }

        protected INavigationService NavigationService { get; }

        protected ILogger Logger { get; }

        public AsyncOperationScope OperationScope { get; }

        public ICommand NavigateBackCommand { get; }

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

        protected bool IsSecurePage { get; set; }

        protected bool IsCurrent => ((RewriteMeNavigationPage)Application.Current.MainPage).CurrentPage.BindingContext.GetType() == GetType();

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            await HandleWebServiceCallAsync(async () =>
            {
                if (IsSecurePage)
                {
                    var isSignedInAsync = await UserSessionService.IsSignedInAsync().ConfigureAwait(false);
                    if (!isSignedInAsync)
                        throw new UnauthorizedCallException();
                }

                await LoadDataAsync(parameters).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        protected async Task HandleWebServiceCallAsync(Func<Task> action)
        {
            try
            {
                await action().ConfigureAwait(false);
            }
            catch (UnauthorizedCallException)
            {
                await UserSessionService.SignOutAsync().ConfigureAwait(false);
                await NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Login}").ConfigureAwait(false);
            }
        }

        protected virtual async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }

        private async Task ExecuteNavigateBackCommandAsync()
        {
            await NavigationService.GoBackWithoutAnimationAsync().ConfigureAwait(false);
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
