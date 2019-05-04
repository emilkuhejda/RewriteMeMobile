using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IRewriteMeWebService _rewriteMeWebService;

        public UserSubscriptionService(
            IInternalValueService internalValueService,
            IUserSubscriptionRepository userSubscriptionRepository,
            IRewriteMeWebService rewriteMeWebService)
        {
            _internalValueService = internalValueService;
            _userSubscriptionRepository = userSubscriptionRepository;
            _rewriteMeWebService = rewriteMeWebService;
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastUserSubscriptionSynchronization = await _internalValueService.GetValueAsync(InternalValues.UserSubscriptionSynchronization).ConfigureAwait(false);
            if (applicationUpdateDate >= lastUserSubscriptionSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetUserSubscriptionsAsync(lastUserSubscriptionSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    await _userSubscriptionRepository.InsertOrReplaceAllAsync(httpRequestResult.Payload).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.UserSubscriptionSynchronization, DateTime.UtcNow).ConfigureAwait(false);
                }
            }
        }

        public async Task<TimeSpan> GetRemainingTimeAsync()
        {
            var userSubscriptionsTime = await _userSubscriptionRepository.GetTotalTimeAsync().ConfigureAwait(false);

            return userSubscriptionsTime;
        }
    }
}
