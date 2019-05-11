using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Mobile.ViewModels
{
    public class UserSubscriptionsPageViewModel : ViewModelBase
    {
        private readonly ISubscriptionProductService _subscriptionProductService;

        private IList<SubscriptionProductViewModel> _subscriptionProducts;

        public UserSubscriptionsPageViewModel(
            ISubscriptionProductService subscriptionProductService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _subscriptionProductService = subscriptionProductService;

            CanGoBack = true;
        }

        public IList<SubscriptionProductViewModel> SubscriptionProducts
        {
            get => _subscriptionProducts;
            private set => SetProperty(ref _subscriptionProducts, value);
        }

        protected override async Task LoadDataAsync(INavigationParameters navigationParameters)
        {
            using (new OperationMonitor(OperationScope))
            {
                var subscriptionProducts = await _subscriptionProductService.GetAsync().ConfigureAwait(false);
                SubscriptionProducts = subscriptionProducts.Select(x => new SubscriptionProductViewModel(x)).ToList();
            }
        }
    }
}
