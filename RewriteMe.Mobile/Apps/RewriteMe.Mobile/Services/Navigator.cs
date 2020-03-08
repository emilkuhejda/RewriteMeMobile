using System;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.ViewModels;
using RewriteMe.Mobile.Views;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Services
{
    public class Navigator : INavigator
    {
        private readonly INavigationService _navigationService;

        public Navigator(INavigationService navigationService)
        {
            _navigationService = navigationService;

            CurrentPage = RootPage.Overview;
        }

        public event EventHandler NavigationReset;

        public RootPage CurrentPage { get; private set; }

        public async Task NavigateToAsync(string name, RootPage rootPage, INavigationParameters navigationParameters = null)
        {
            var navigationPage = Application.Current.MainPage as RewriteMeNavigationPage;
            navigationPage?.Pages.ForEach(x => ((ViewModelBase)x.BindingContext).Dispose());

            CurrentPage = rootPage;

            await _navigationService.NavigateWithoutAnimationAsync(name, navigationParameters).ConfigureAwait(false);
        }

        public void ResetNavigation()
        {
            CurrentPage = RootPage.Overview;

            OnNavigationReset();
        }

        private void OnNavigationReset()
        {
            NavigationReset?.Invoke(this, EventArgs.Empty);
        }
    }
}
