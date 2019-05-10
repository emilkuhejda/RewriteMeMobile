using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class SubscriptionProductService : ISubscriptionProductService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly ISubscriptionProductRepository _subscriptionProductRepository;
        private readonly ILogger _logger;

        public SubscriptionProductService(
            IInternalValueService internalValueService,
            IRewriteMeWebService rewriteMeWebService,
            ISubscriptionProductRepository subscriptionProductRepository,
            ILoggerFactory loggerFactory)
        {
            _internalValueService = internalValueService;
            _rewriteMeWebService = rewriteMeWebService;
            _subscriptionProductRepository = subscriptionProductRepository;
            _logger = loggerFactory.CreateLogger(typeof(SubscriptionProductService));
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastSubscriptionProductSynchronization = await _internalValueService.GetValueAsync(InternalValues.SubscriptionProductSynchronization).ConfigureAwait(false);
            _logger.Debug($"Update subscription products with timestamp '{lastSubscriptionProductSynchronization.ToString("d", CultureInfo.InvariantCulture)}'.");

            if (applicationUpdateDate >= lastSubscriptionProductSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetSubscriptionProductsAsync().ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    await _subscriptionProductRepository.ReplaceAllAsync(httpRequestResult.Payload).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.SubscriptionProductSynchronization, DateTime.UtcNow).ConfigureAwait(false);
                }
            }
        }

        public async Task<IEnumerable<SubscriptionProduct>> GetAsync()
        {
            return await _subscriptionProductRepository.GetAsync().ConfigureAwait(false);
        }
    }
}
