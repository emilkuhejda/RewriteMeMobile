using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Transcription;

namespace RewriteMe.Mobile.ViewModels
{
    public class UserSubscriptionsPageViewModel : ViewModelBase
    {
        private IList<SubscriptionProductViewModel> _products;

        public UserSubscriptionsPageViewModel(
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            CanGoBack = true;
        }

        public IList<SubscriptionProductViewModel> Products
        {
            get => _products;
            private set => SetProperty(ref _products, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                Products = SubscriptionProducts.All.Select(x => new SubscriptionProductViewModel(x)).ToList();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }
    }
}
