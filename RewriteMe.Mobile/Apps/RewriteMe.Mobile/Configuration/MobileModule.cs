using Prism.Ioc;
using RewriteMe.Common;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.ViewModels;
using RewriteMe.Mobile.Views;

namespace RewriteMe.Mobile.Configuration
{
    public class MobileModule : IUnityModule
    {
        public void RegisterServices(IContainerRegistry containerRegistry)
        {
            RegisterPages(containerRegistry);
        }

        private static void RegisterPages(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<RewriteMeNavigationPage>(Pages.Navigation);
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>(Pages.Login);
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>(Pages.Main);
        }
    }
}
