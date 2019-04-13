using Prism.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public SettingsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            CanGoBack = true;
        }
    }
}
