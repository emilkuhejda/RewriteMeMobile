using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Unity;
using RewriteMe.Business.Configuration;
using RewriteMe.Common.Utils;
using RewriteMe.DataAccess;
using RewriteMe.DataAccess.Providers;
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
        public App(IPlatformInitializer platformInitializer)
            : base(platformInitializer)
        {
        }

        public void ImportFile(string absolutePath, string path)
        {
            var navigationParameters = CreateNavigationParameters(absolutePath, path);
            NavigationService.NavigateAsync($"{Pages.Login}", navigationParameters);
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            InitializeExperimentalFeatures();
            InitializeStorage();

            var navigationParameters = CreateNavigationParameters(InitializationParameters.Current.ImportedFilePath);
            NavigationService.NavigateAsync($"/{Pages.Login}", navigationParameters);
        }

        private NavigationParameters CreateNavigationParameters(string filePath)
        {
            return CreateNavigationParameters(filePath, filePath);
        }

        private NavigationParameters CreateNavigationParameters(string absolutePath, string filePath)
        {
            var navigationParameters = new NavigationParameters();
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                var importedFileNavigationParameters = new ImportedFileNavigationParameters(absolutePath, filePath);
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

        protected override void OnResume()
        {
            Container.Resolve<ISchedulerService>().StartAsync();
        }

        protected override async void OnSleep()
        {
            Container.Resolve<ISchedulerService>().Stop();
            await Container.Resolve<IAppDbContextProvider>().CloseAsync().ConfigureAwait(false);
        }
    }
}
