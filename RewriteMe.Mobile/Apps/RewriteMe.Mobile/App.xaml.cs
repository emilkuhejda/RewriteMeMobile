using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Unity;
using RewriteMe.Business.Configuration;
using RewriteMe.Common.Utils;
using RewriteMe.DataAccess;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Mobile.Configuration;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using Syncfusion.Licensing;
using Unity;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace RewriteMe.Mobile
{
    public partial class App : PrismApplication
    {
        private IApplicationSettings _applicationSettings;

        public App(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
        {
        }

        public void ImportFile(string fileName, byte[] source)
        {
            var navigationParameters = CreateNavigationParameters(fileName, source);
            NavigateToPage(navigationParameters);
        }

        protected override void OnInitialized()
        {
            _applicationSettings = Container.Resolve<IApplicationSettings>();

            SyncfusionLicenseProvider.RegisterLicense(_applicationSettings.SyncfusionKey);
            Locator.SetContainerProvider(Container);

            InitializeComponent();

            InitializeServices();
            ResetFileItemUploadStatusesAsync();

            var navigationParameters = InitializeNavigationParameters();
            NavigateToPage(navigationParameters);
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            var container = new UnityContainer().AddExtension(new ForceActivation());
            return new UnityContainerExtension(container);
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

        private void ResetFileItemUploadStatusesAsync()
        {
            AsyncHelper.RunSync(() => Container.Resolve<IFileItemService>().ResetUploadStatusesAsync());
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

            Container.Resolve<IAuthorizationObserver>().Initialize(NavigationService);
        }

        private void NavigateToPage(INavigationParameters navigationParameters)
        {
            var alreadySignedIn = AsyncHelper.RunSync(() => Container.Resolve<IUserSessionService>().IsSignedInAsync());
            if (alreadySignedIn)
            {
                var name = navigationParameters.ContainsKey(nameof(ImportedFileNavigationParameters))
                    ? $"/{Pages.Navigation}/{Pages.Overview}/{Pages.Create}"
                    : $"/{Pages.Navigation}/{Pages.Overview}";

                NavigationService.NavigateWithoutAnimationAsync(name, navigationParameters).ConfigureAwait(false);
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
                        await Container.Resolve<IAuthorizationObserver>().LogOutAsync().ConfigureAwait(false);
                    }
                };
            }

#if !DEBUG
            AppCenter.Start(_applicationSettings.AppCenterKeys, typeof(Crashes), typeof(Analytics), typeof(Push));|
#else
            AppCenter.Start(_applicationSettings.AppCenterKeys, typeof(Push));
#endif
        }

        protected override async void OnResume()
        {
            var navigationParameters = InitializeNavigationParameters();
            if (navigationParameters.ContainsKey(nameof(ImportedFileNavigationParameters)))
            {
                var name = $"{Pages.Overview}/{Pages.Create}";
                await NavigationService.NavigateWithoutAnimationAsync(name, navigationParameters).ConfigureAwait(false);
            }
        }

        protected override async void OnSleep()
        {
            await Container.Resolve<IAppDbContextProvider>().CloseAsync().ConfigureAwait(false);
        }
    }
}
