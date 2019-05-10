using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Services
{
    public class LastUpdatesService : ILastUpdatesService
    {
        private readonly IRewriteMeWebService _rewriteMeWebService;

        private LastUpdates _lastUpdates;
        private bool _isInitialized;

        public LastUpdatesService(IRewriteMeWebService rewriteMeWebService)
        {
            _rewriteMeWebService = rewriteMeWebService;
        }

        public bool IsConnectionSuccessful { get; private set; }

        public async Task InitializeAsync()
        {
            var httpRequestResult = await _rewriteMeWebService.GetLastUpdates().ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                _lastUpdates = httpRequestResult.Payload;

                IsConnectionSuccessful = true;
            }

            _isInitialized = true;
        }

        public DateTime GetFileItemLastUpdate()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Service is not initialized");

            return _lastUpdates?.FileItem ?? DateTime.MinValue;
        }

        public DateTime GetTranscribeItemLastUpdate()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Service is not initialized");

            return _lastUpdates?.TranscribeItem ?? DateTime.MinValue;
        }

        public DateTime GetUserSubscriptionLastUpdate()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Service is not initialized");

            return _lastUpdates?.UserSubscription ?? DateTime.MinValue;
        }

        public DateTime GetSubscriptionProductLastUpdate()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Service is not initialized");

            return _lastUpdates?.SubscriptionProduct ?? DateTime.MinValue;
        }
    }
}
