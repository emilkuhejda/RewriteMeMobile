using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IInternalValueService _internalValueService;

        public UserSubscriptionService(IInternalValueService internalValueService)
        {
            _internalValueService = internalValueService;
        }

        public async Task SubtractTimeAsync(TimeSpan time)
        {
            var remainingTimeTicks = await _internalValueService.GetValueAsync(InternalValues.RemainingTimeTicks).ConfigureAwait(false);
            var currentTicks = remainingTimeTicks - time.Ticks;

            await UpdateRemainingTimeAsync(TimeSpan.FromTicks(currentTicks)).ConfigureAwait(false);
        }

        public async Task UpdateRemainingTimeAsync(TimeSpan remainingTime)
        {
            await _internalValueService.UpdateValueAsync(InternalValues.RemainingTimeTicks, remainingTime.Ticks).ConfigureAwait(false);
        }

        public async Task<TimeSpan> GetRemainingTimeAsync()
        {
            var remainingTime = await _internalValueService.GetValueAsync(InternalValues.RemainingTimeTicks).ConfigureAwait(false);
            return TimeSpan.FromTicks(remainingTime);
        }
    }
}
