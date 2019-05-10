using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Services
{
    public class BillingPurchaseService : IBillingPurchaseService
    {
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IUserSessionService _userSessionService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IBillingPurchaseRepository _billingPurchaseRepository;

        public BillingPurchaseService(
            IUserSubscriptionService userSubscriptionService,
            IUserSessionService userSessionService,
            IRewriteMeWebService rewriteMeWebService,
            IBillingPurchaseRepository billingPurchaseRepository)
        {
            _userSubscriptionService = userSubscriptionService;
            _userSessionService = userSessionService;
            _rewriteMeWebService = rewriteMeWebService;
            _billingPurchaseRepository = billingPurchaseRepository;
        }

        public async Task SendBillingPurchaseAsync(BillingPurchase billingPurchase)
        {
            var httpRequestResult = await _rewriteMeWebService.CreateUserSubscriptionAsync(billingPurchase).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                await _userSubscriptionService.AddAsync(httpRequestResult.Payload).ConfigureAwait(false);
            }
            else
            {
                await _billingPurchaseRepository.AddAsync(billingPurchase).ConfigureAwait(false);

                if (httpRequestResult.State == HttpRequestState.Error)
                {
                    throw new ErrorRequestException(httpRequestResult.StatusCode);
                }

                throw new OfflineRequestException();
            }
        }

        public async Task SendPendingBillingPurchasesAsync()
        {
            var userId = await _userSessionService.GetUserIdAsync().ConfigureAwait(false);
            var billingPurchases = await _billingPurchaseRepository.GetAllAsync(userId).ConfigureAwait(false);
            var pendingBillingPurchases = billingPurchases.ToList();
            if (!pendingBillingPurchases.Any())
                return;

            var tasks = new List<Func<Task>>();
            foreach (var billingPurchase in pendingBillingPurchases)
            {
                tasks.Add(() => SendPendingBillingPurchaseAsync(billingPurchase));
            }

            await Task.WhenAll(tasks.Select(x => x()).ToList()).ConfigureAwait(false);
        }

        private async Task SendPendingBillingPurchaseAsync(BillingPurchase billingPurchase)
        {
            var httpRequestResult = await _rewriteMeWebService.CreateUserSubscriptionAsync(billingPurchase).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                await _billingPurchaseRepository.DeleteAsync(billingPurchase.Id.GetValueOrDefault()).ConfigureAwait(false);
            }
        }
    }
}
