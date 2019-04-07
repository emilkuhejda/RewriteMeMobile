using Prism;
using Prism.Ioc;
using Prism.Unity;
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

            NavigationService.NavigateAsync($"/{Pages.Login}");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
