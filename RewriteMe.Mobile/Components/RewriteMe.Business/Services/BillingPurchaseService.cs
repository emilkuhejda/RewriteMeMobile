using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.InAppBilling;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Services
{
    public class BillingPurchaseService : IBillingPurchaseService
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IConnectivityService _connectivityService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IBillingPurchaseRepository _billingPurchaseRepository;

        public BillingPurchaseService(
            IUserSessionService userSessionService,
            IUserSubscriptionService userSubscriptionService,
            IConnectivityService connectivityService,
            IRewriteMeWebService rewriteMeWebService,
            IBillingPurchaseRepository billingPurchaseRepository)
        {
            _userSessionService = userSessionService;
            _userSubscriptionService = userSubscriptionService;
            _connectivityService = connectivityService;
            _rewriteMeWebService = rewriteMeWebService;
            _billingPurchaseRepository = billingPurchaseRepository;
        }

        public Task<IEnumerable<InAppBillingPurchase>> GetAllPaymentPendingAsync()
        {
            return _billingPurchaseRepository.GetAllPaymentPendingAsync();
        }

        public Task AddAsync(InAppBillingPurchase billingPurchase)
        {
            return _billingPurchaseRepository.AddAsync(billingPurchase);
        }

        public async Task<TimeSpanWrapper> SendBillingPurchaseAsync(CreateUserSubscriptionInputModel createUserSubscriptionInputModel)
        {
            var httpRequestResult = await _rewriteMeWebService.CreateUserSubscriptionAsync(createUserSubscriptionInputModel).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                return httpRequestResult.Payload;
            }

            if (httpRequestResult.State == HttpRequestState.Error)
            {
                throw new ErrorRequestException(httpRequestResult.ErrorCode);
            }

            throw new OfflineRequestException();
        }

        public async Task HandlePendingPurchases()
        {
            if (!_connectivityService.IsConnected)
                return;

            if (!CrossInAppBilling.IsSupported)
                return;

            var pendingPurchasesEnumerable = await _billingPurchaseRepository.GetAllPaymentPendingAsync().ConfigureAwait(false);
            var pendingPurchases = pendingPurchasesEnumerable.ToList();
            if (!pendingPurchases.Any())
                return;

            try
            {
                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync().ConfigureAwait(false);
                if (!connected)
                    throw new AppStoreNotConnectedException();

                var userId = await _userSessionService.GetUserIdAsync().ConfigureAwait(false);
                var purchasesEnumerable = await billing.GetPurchasesAsync(ItemType.InAppPurchase).ConfigureAwait(false);
                var purchases = purchasesEnumerable?.ToList();
                if (purchases == null || !purchases.Any())
                {
                    var isSuccessList = new List<bool>();
                    foreach (var deprecatedPurchase in pendingPurchases)
                    {
                        var isConsumed = await ConsumePurchaseAsync(billing, deprecatedPurchase, userId).ConfigureAwait(false);
                        isSuccessList.Add(isConsumed);
                    }

                    if (isSuccessList.All(x => x))
                        return;

                    throw new NoPurchasesInStoreException();
                }

                foreach (var pendingPurchase in pendingPurchases)
                {
                    var purchase = purchases.FirstOrDefault(x => x.Id.Equals(pendingPurchase.Id, StringComparison.OrdinalIgnoreCase));
                    if (purchase == null)
                    {
                        var isConsumed = await ConsumePurchaseAsync(billing, pendingPurchase, userId).ConfigureAwait(false);
                        if (isConsumed)
                            continue;

                        throw new PurchaseNotFoundException(pendingPurchase.Id, pendingPurchase.ProductId);
                    }

                    if (purchase.State == PurchaseState.Purchased || purchase.State == PurchaseState.PaymentPending)
                    {
                        await ConsumePurchaseAsync(billing, purchase, userId).ConfigureAwait(false);
                    }
                }
            }
            finally
            {
                await CrossInAppBilling.Current.DisconnectAsync().ConfigureAwait(false);
            }
        }

        private async Task<bool> ConsumePurchaseAsync(IInAppBilling billing, InAppBillingPurchase pendingPurchase, Guid userId)
        {
            var isConsumed = false;
            try
            {
                isConsumed = await billing.ConsumePurchaseAsync(pendingPurchase.ProductId, pendingPurchase.PurchaseToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                await CheckBillingPurchaseAsync(billing, pendingPurchase, userId).ConfigureAwait(false);
            }

            if (isConsumed)
            {
                try
                {
                    var orderId = pendingPurchase.Id;
                    pendingPurchase.ConsumptionState = ConsumptionState.Consumed;
                    pendingPurchase.State = PurchaseState.Purchased;

                    var billingPurchase = pendingPurchase.ToUserSubscriptionModel(userId, orderId);
                    var remainingTime = await SendBillingPurchaseAsync(billingPurchase).ConfigureAwait(false);

                    await _userSubscriptionService.UpdateRemainingTimeAsync(remainingTime.Time).ConfigureAwait(false);
                    await _billingPurchaseRepository.UpdateAsync(pendingPurchase).ConfigureAwait(false);
                    return true;
                }
                catch (OfflineRequestException ex)
                {
                    throw new RegistrationPurchaseBillingException(pendingPurchase.Id, pendingPurchase.ProductId, nameof(pendingPurchase), ex);
                }
                catch (ErrorRequestException ex)
                {
                    throw new RegistrationPurchaseBillingException(pendingPurchase.Id, pendingPurchase.ProductId, nameof(pendingPurchase), ex);
                }
            }

            return false;
        }

        private async Task CheckBillingPurchaseAsync(IInAppBilling billing, InAppBillingPurchase pendingPurchase, Guid userId)
        {
            try
            {
                var inAppBillingPurchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase).ConfigureAwait(false);
                var purchase = inAppBillingPurchases.SingleOrDefault(x => x.Id.Equals(pendingPurchase.Id, StringComparison.OrdinalIgnoreCase));
                if (purchase == null)
                {
                    var orderId = pendingPurchase.Id;
                    pendingPurchase.State = PurchaseState.Failed;

                    var billingPurchase = pendingPurchase.ToUserSubscriptionModel(userId, orderId);
                    var remainingTime = await SendBillingPurchaseAsync(billingPurchase).ConfigureAwait(false);

                    await _userSubscriptionService.UpdateRemainingTimeAsync(remainingTime.Time).ConfigureAwait(false);
                    await _billingPurchaseRepository.UpdateAsync(pendingPurchase).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                // Ignored
            }
        }
    }
}
