using Plugin.DeviceInfo;
using Plugin.LatestVersion;
using Plugin.Messaging;
using Prism.Ioc;
using RewriteMe.Common;
using RewriteMe.Domain.Interfaces.ExceptionHandling;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Mobile.ExceptionHandling;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Services;
using RewriteMe.Mobile.ViewModels;
using RewriteMe.Mobile.Views;

namespace RewriteMe.Mobile.Configuration
{
    public class MobileModule : IUnityModule
    {
        public void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(CrossDevice.Hardware);
            containerRegistry.RegisterInstance(CrossLatestVersion.Current);
            containerRegistry.RegisterInstance(CrossMessaging.Current.EmailMessenger);
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            containerRegistry.RegisterSingleton<IExceptionHandlingStrategy, ExceptionHandlingStrategy>();

            RegisterPages(containerRegistry);
        }

        private static void RegisterPages(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<RewriteMeNavigationPage>(Pages.Navigation);
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>(Pages.Login);
            containerRegistry.RegisterForNavigation<LoadingPage, LoadingPageViewModel>(Pages.Loading);
            containerRegistry.RegisterForNavigation<OverviewPage, OverviewPageViewModel>(Pages.Overview);
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>(Pages.Settings);
            containerRegistry.RegisterForNavigation<UserSettingsPage, UserSettingsPageViewModel>(Pages.UserSettings);
            containerRegistry.RegisterForNavigation<DeveloperPage, DeveloperPageViewModel>(Pages.Developer);
            containerRegistry.RegisterForNavigation<DropDownListPage, DropDownListPageViewModel>(Pages.DropDownListPage);
        }
    }
}
