using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(
            IDialogService dialogService,
            INavigationService navigationService)
            : base(dialogService, navigationService)
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
