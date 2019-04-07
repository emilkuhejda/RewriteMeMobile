using Prism.Navigation;
using RewriteMe.Resources.Localization;

namespace RewriteMe.Mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public LoginPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            CanGoBack = true;
            Title = Loc.Text(TranslationKeys.LoginPageTitle);
        }
    }
}
