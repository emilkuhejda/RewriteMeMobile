using Prism;
using Prism.Ioc;
using Prism.Unity;
using RewriteMe.Common.Utils;
using RewriteMe.DataAccess;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Mobile.Navigation;
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

        protected override void OnInitialized()
        {
            InitializeComponent();

            InitializeStorage();

            NavigationService.NavigateAsync($"/{Pages.Login}");
        }

        private void InitializeStorage()
        {
            // Move to OnStart method
            AsyncHelper.RunSync(() => Container.Resolve<IStorageInitializer>().InitializeAsync());
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override async void OnSleep()
        {
            await Container.Resolve<IAppDbContextProvider>().CloseAsync().ConfigureAwait(false);
        }
    }
}
