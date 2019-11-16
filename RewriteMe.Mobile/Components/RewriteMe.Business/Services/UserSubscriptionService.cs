using System;
using System.Globalization;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly ILogger _logger;

        public UserSubscriptionService(
            IInternalValueService internalValueService,
            IRewriteMeWebService rewriteMeWebService,
            ILoggerFactory loggerFactory)
        {
            _internalValueService = internalValueService;
            _rewriteMeWebService = rewriteMeWebService;
            _logger = loggerFactory.CreateLogger(typeof(UserSubscriptionService));
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastUserSubscriptionSynchronizationTicks = await _internalValueService.GetValueAsync(InternalValues.UserSubscriptionSynchronizationTicks).ConfigureAwait(false);
            var lastUserSubscriptionSynchronization = new DateTime(lastUserSubscriptionSynchronizationTicks);
            _logger.Debug($"Update user subscription remaining time with timestamp '{lastUserSubscriptionSynchronization.ToString("d", CultureInfo.InvariantCulture)}'.");

            if (applicationUpdateDate >= lastUserSubscriptionSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetUserSubscriptionRemainingTimeAsync().ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    _logger.Info("Web server returned response for synchronization.");

                    await UpdateRemainingTimeAsync(httpRequestResult.Payload.Time).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.UserSubscriptionSynchronizationTicks, DateTime.UtcNow.Ticks).ConfigureAwait(false);
                }
            }
        }

        public async Task RecognizedTimeSynchronizationAsync()
        {
            var httpRequestResult = await _rewriteMeWebService.GetRecognizedTimeAsync().ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                await _internalValueService
                    .UpdateValueAsync(InternalValues.RecognizedTimeTicks, httpRequestResult.Payload.TotalTime.Ticks)
                    .ConfigureAwait(false);
            }
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
