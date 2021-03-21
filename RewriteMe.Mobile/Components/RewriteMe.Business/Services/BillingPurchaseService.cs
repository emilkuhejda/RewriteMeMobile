using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.InAppBilling;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Services
{
    public class BillingPurchaseService : IBillingPurchaseService
    {
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IBillingPurchaseRepository _billingPurchaseRepository;

        public BillingPurchaseService(
            IRewriteMeWebService rewriteMeWebService,
            IBillingPurchaseRepository billingPurchaseRepository)
        {
            _rewriteMeWebService = rewriteMeWebService;
            _billingPurchaseRepository = billingPurchaseRepository;
        }

        public Task<IEnumerable<InAppBillingPurchase>> GetAllAsync()
        {
            return _billingPurchaseRepository.GetAllAsync();
        }

        public Task<IEnumerable<InAppBillingPurchase>> GetAllPaymentPendingAsync()
        {
            return _billingPurchaseRepository.GetAllPaymentPendingAsync();
        }

        public Task AddAsync(InAppBillingPurchase billingPurchase)
        {
            return _billingPurchaseRepository.AddAsync(billingPurchase);
        }

        public Task UpdateAsync(InAppBillingPurchase billingPurchase)
        {
            return _billingPurchaseRepository.UpdateAsync(billingPurchase);
        }

        public Task DeleteAsync(string billingPurchaseId)
        {
            return _billingPurchaseRepository.DeleteAsync(billingPurchaseId);
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
    }
}
