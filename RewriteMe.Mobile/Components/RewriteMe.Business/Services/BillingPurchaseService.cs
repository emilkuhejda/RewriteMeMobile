using System.Threading.Tasks;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Services
{
    public class BillingPurchaseService : IBillingPurchaseService
    {
        private readonly IRewriteMeWebService _rewriteMeWebService;

        public BillingPurchaseService(IRewriteMeWebService rewriteMeWebService)
        {
            _rewriteMeWebService = rewriteMeWebService;
        }

        public async Task<RemainingTime> SendBillingPurchaseAsync(BillingPurchase billingPurchase)
        {
            var httpRequestResult = await _rewriteMeWebService.CreateUserSubscriptionAsync(billingPurchase).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                return httpRequestResult.Payload;
            }

            if (httpRequestResult.State == HttpRequestState.Error)
            {
                throw new ErrorRequestException(httpRequestResult.StatusCode);
            }

            throw new OfflineRequestException();
        }
    }
}
