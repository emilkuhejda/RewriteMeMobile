using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Unity;
using RewriteMe.Business.Configuration;
using RewriteMe.Common.Utils;
using RewriteMe.DataAccess;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;
using RewriteMe.Mobile.Navigation.Parameters;
using Xamarin.Essentials;
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

            NavigationService.NavigateAsync($"{Pages.Login}", navigationParameters);
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            InitializeExperimentalFeatures();
            InitializeStorage();

            var navigationParameters = InitializeNavigationParameters();
            NavigationService.NavigateAsync($"/{Pages.Login}", navigationParameters);
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

        private void InitializeExperimentalFeatures()
        {
            ExperimentalFeatures.Enable(ExperimentalFeatures.EmailAttachments);
        }

        private void InitializeStorage()
        {
            AsyncHelper.RunSync(() => Container.Resolve<IStorageInitializer>().InitializeAsync());
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void OnStart()
        {
            base.OnStart();

            AppCenter.Start(_applicationSettings.AppCenterKeys, typeof(Crashes));
        }

        protected override void OnResume()
        {
            Container.Resolve<ISchedulerService>().Start();
        }

        protected override async void OnSleep()
        {
            Container.Resolve<ISchedulerService>().Stop();
            await Container.Resolve<IAppDbContextProvider>().CloseAsync().ConfigureAwait(false);
        }
    }
}
