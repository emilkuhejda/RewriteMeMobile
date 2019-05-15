using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Interfaces;
using RewriteMe.Mobile.Transcription;

namespace RewriteMe.Mobile.ViewModels
{
    public class UserSubscriptionsPageViewModel : ViewModelBase
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IBillingPurchaseService _billingPurchaseService;
        private readonly ILoggerFactory _loggerFactory;

        private IList<SubscriptionProductViewModel> _products;

        public UserSubscriptionsPageViewModel(
            IUserSessionService userSessionService,
            IUserSubscriptionService userSubscriptionService,
            IBillingPurchaseService billingPurchaseService,
            IDialogService dialogService,
            INavigationService navigationService,
            ILoggerFactory loggerFactory)
            : base(dialogService, navigationService, loggerFactory)
        {
            _userSessionService = userSessionService;
            _userSubscriptionService = userSubscriptionService;
            _billingPurchaseService = billingPurchaseService;
            _loggerFactory = loggerFactory;

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
                Products = SubscriptionProducts.All
                    .Select(x => new SubscriptionProductViewModel(x, _userSessionService, _userSubscriptionService, _billingPurchaseService, DialogService, _loggerFactory))
                    .ToList();

                await Task.CompletedTask.ConfigureAwait(false);
            }
        }
    }
}
