using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Unity;
using RewriteMe.Common.Utils;
using RewriteMe.DataAccess;
using RewriteMe.DataAccess.Providers;
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
        public App(IPlatformInitializer platformInitializer)
        : base(platformInitializer)
        {
        }

        public void CreateFileItem(string path)
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add<ImportedFileNavigationParameters>(new ImportedFileNavigationParameters(path));
            NavigationService.NavigateAsync($"{Pages.Login}", navigationParameters);
        }

        protected override void OnInitialized()
        {
            InitializeComponent();

            InitializeStorage();

            NavigationService.NavigateAsync($"/{Pages.Login}");
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
