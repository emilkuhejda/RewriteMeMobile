using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;

namespace RewriteMe.Mobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            CanGoBack = true;
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }
        }
    }
}
