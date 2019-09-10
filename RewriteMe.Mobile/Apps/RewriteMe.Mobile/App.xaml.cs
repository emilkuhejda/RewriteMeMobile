using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Unity;
using RewriteMe.Business.Configuration;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.DataAccess;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace RewriteMe.Mobile
{
    public partial class App : PrismApplication
    {
        private readonly IApplicationSettings _applicationSettings;

        public App(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
        {
            _applicationSettings = Container.Resolve<IApplicationSettings>();
        }

        public void ImportFile(string fileName, byte[] source)
        {
            var navigationParameters = CreateNavigationParameters(fileName, source);
            NavigateToPage(navigationParameters);
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            InitializeServices();
            InitializeManagers();

            var navigationParameters = InitializeNavigationParameters();
            NavigateToPage(navigationParameters);
        }

        private INavigationParameters InitializeNavigationParameters()
        {
            var initializationParameters = InitializationParameters.Current;
            var navigationParameters = CreateNavigationParameters(initializationParameters.ImportedFileName, initializationParameters.ImportedFileSource);
            initializationParameters.Clear();

            return navigationParameters;
        }

        private NavigationParameters CreateNavigationParameters(string fileName, byte[] source)
        {
            var navigationParameters = new NavigationParameters();
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var importedFileNavigationParameters = new ImportedFileNavigationParameters(fileName, source);
                navigationParameters.Add<ImportedFileNavigationParameters>(importedFileNavigationParameters);
            }

            return navigationParameters;
        }

        private void InitializeServices()
        {
            AsyncHelper.RunSync(InitializeServicesAsync);
        }

        private void InitializeManagers()
        {
            Container.Resolve<IBackgroundTasksManager>().Initialize();
        }

        private async Task InitializeServicesAsync()
        {
            await Container.Resolve<IStorageInitializer>().InitializeAsync().ConfigureAwait(false);

            var tasks = new List<Func<Task>>
            {
                Container.Resolve<IApplicationSettings>().InitializeAsync,
                Container.Resolve<ILanguageService>().InitializeAsync
            };

            Task.WaitAll(tasks.Select(x => x()).ToArray());
        }

        private void NavigateToPage(INavigationParameters navigationParameters)
        {
            var alreadySignedIn = AsyncHelper.RunSync(() => Container.Resolve<IUserSessionService>().IsSignedInAsync());
            if (alreadySignedIn)
            {
                NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Navigation}/{Pages.Overview}", navigationParameters).ConfigureAwait(false);
            }
            else
            {
                NavigationService.NavigateWithoutAnimationAsync($"/{Pages.Login}", navigationParameters).ConfigureAwait(false);
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (!AppCenter.Configured)
            {
                Push.PushNotificationReceived += async (sender, e) =>
                {
                    try
                    {
                        await Container.Resolve<ISynchronizationService>().StartAsync().ConfigureAwait(false);
                    }
                    catch (UnauthorizedCallException)
                    {
                    }
                };
            }

            AppCenter.Start(_applicationSettings.AppCenterKeys, typeof(Crashes), typeof(Push));
        }

        protected override void OnResume()
        {
            Container.Resolve<ISchedulerService>().Start().FireAndForget();
        }

        protected override async void OnSleep()
        {
            Container.Resolve<ISchedulerService>().Stop();
            await Container.Resolve<IAppDbContextProvider>().CloseAsync().ConfigureAwait(false);
        }
    }
}
