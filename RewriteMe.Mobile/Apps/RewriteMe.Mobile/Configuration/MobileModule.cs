using Plugin.Connectivity;
using Plugin.LatestVersion;
using Plugin.Messaging;
using Prism.Ioc;
using Prism.Navigation;
using RewriteMe.Common;
using RewriteMe.Domain.Interfaces.ExceptionHandling;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Mobile.ExceptionHandling;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Services;
using RewriteMe.Mobile.ViewModels;
using RewriteMe.Mobile.Views;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Configuration
{
    public class MobileModule : IUnityModule
    {
        public void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(CrossLatestVersion.Current);
            containerRegistry.RegisterInstance(CrossMessaging.Current.EmailMessenger);
            containerRegistry.RegisterInstance(CrossConnectivity.Current);
            containerRegistry.RegisterInstance(MessagingCenter.Instance);

            containerRegistry.RegisterSingleton<INavigationService, PageNavigationService>();
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            containerRegistry.RegisterSingleton<ILanguageService, LanguageService>();
            containerRegistry.RegisterSingleton<IEmailService, EmailService>();
            containerRegistry.RegisterSingleton<IAuthorizationObserver, AuthorizationObserver>();
            containerRegistry.RegisterSingleton<IAppCenterMetricsService, AppCenterMetricsService>();
            containerRegistry.RegisterSingleton<IPushNotificationsService, PushNotificationsService>();
            containerRegistry.RegisterSingleton<INavigator, Navigator>();
            containerRegistry.RegisterSingleton<IExceptionHandlingStrategy, ExceptionHandlingStrategy>();

            RegisterViewModels(containerRegistry);
            RegisterPages(containerRegistry);
        }

        private static void RegisterViewModels(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<BottomNavigationViewModel>();
        }

        private static void RegisterPages(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<RewriteMeNavigationPage>(Pages.Navigation);
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>(Pages.Login);
            containerRegistry.RegisterForNavigation<LoadingPage, LoadingPageViewModel>(Pages.Loading);
            containerRegistry.RegisterForNavigation<IntroPage, IntroPageViewModel>(Pages.Intro);
            containerRegistry.RegisterForNavigation<OverviewPage, OverviewPageViewModel>(Pages.Overview);
            containerRegistry.RegisterForNavigation<RecorderOverviewPage, RecorderOverviewPageViewModel>(Pages.RecorderOverview);
            containerRegistry.RegisterForNavigation<InfoOverviewPage, InfoOverviewPageViewModel>(Pages.InfoOverview);
            containerRegistry.RegisterForNavigation<DetailInfoPage, DetailInfoPageViewModel>(Pages.DetailInfo);
            containerRegistry.RegisterForNavigation<RecorderPage, RecorderPageViewModel>(Pages.Recorder);
            containerRegistry.RegisterForNavigation<CreatePage, CreatePageViewModel>(Pages.Create);
            containerRegistry.RegisterForNavigation<TranscribePage, TranscribePageViewModel>(Pages.Transcribe);
            containerRegistry.RegisterForNavigation<TranscribeRecodingPage, TranscribeRecodingPageViewModel>(Pages.TranscribeRecoding);
            containerRegistry.RegisterForNavigation<DetailPage, DetailPageViewModel>(Pages.Detail);
            containerRegistry.RegisterForNavigation<RecordedDetailPage, RecordedDetailPageViewModel>(Pages.RecordedDetail);
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>(Pages.Settings);
            containerRegistry.RegisterForNavigation<UserSettingsPage, UserSettingsPageViewModel>(Pages.UserSettings);
            containerRegistry.RegisterForNavigation<UserSubscriptionsPage, UserSubscriptionsPageViewModel>(Pages.UserSubscriptions);
            containerRegistry.RegisterForNavigation<DeveloperPage, DeveloperPageViewModel>(Pages.Developer);
            containerRegistry.RegisterForNavigation<DropDownListPage, DropDownListPageViewModel>(Pages.DropDownListPage);
        }
    }
}
