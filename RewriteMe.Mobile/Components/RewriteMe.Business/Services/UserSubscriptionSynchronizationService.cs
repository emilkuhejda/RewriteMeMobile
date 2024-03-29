﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class UserSubscriptionSynchronizationService : IUserSubscriptionSynchronizationService
    {
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IInternalValueService _internalValueService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly ILogger _logger;

        public UserSubscriptionSynchronizationService(
            IUserSubscriptionService userSubscriptionService,
            IInternalValueService internalValueService,
            IRewriteMeWebService rewriteMeWebService,
            ILoggerFactory loggerFactory)
        {
            _userSubscriptionService = userSubscriptionService;
            _internalValueService = internalValueService;
            _rewriteMeWebService = rewriteMeWebService;
            _logger = loggerFactory.CreateLogger(typeof(UserSubscriptionSynchronizationService));
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastUserSubscriptionSynchronizationTicks = await _internalValueService.GetValueAsync(InternalValues.UserSubscriptionSynchronizationTicks).ConfigureAwait(false);
            var lastUserSubscriptionSynchronization = new DateTime(lastUserSubscriptionSynchronizationTicks);
            _logger.Debug($"Update user subscription remaining time with timestamp '{lastUserSubscriptionSynchronization.ToString("d", CultureInfo.InvariantCulture)}'.");

            if (applicationUpdateDate > lastUserSubscriptionSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetUserSubscriptionRemainingTimeAsync().ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    _logger.Info("Web server returned response for synchronization.");

                    await _userSubscriptionService.UpdateRemainingTimeAsync(httpRequestResult.Payload.Time).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.UserSubscriptionSynchronizationTicks, applicationUpdateDate.Ticks).ConfigureAwait(false);
                }
            }
        }
    }
}
