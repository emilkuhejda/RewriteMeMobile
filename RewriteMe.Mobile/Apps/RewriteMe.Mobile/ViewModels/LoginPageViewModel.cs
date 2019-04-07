using Prism.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public LoginPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            CanGoBack = true;
            Title = "Login Page";
        }
    }
}
