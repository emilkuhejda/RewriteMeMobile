using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Mobile.Extensions;
using RewriteMe.Mobile.Navigation;

namespace RewriteMe.Mobile.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public LoginPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            HasTitleBar = false;
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                await NavigationService.NavigateWithoutAnimationAsync(Pages.Main).ConfigureAwait(false);
            }
        }
    }
}
