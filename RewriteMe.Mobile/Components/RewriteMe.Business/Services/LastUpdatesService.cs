using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi;

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
            var httpRequestResult = await _rewriteMeWebService.GetLastUpdatesAsync().ConfigureAwait(false);
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

            return _lastUpdates?.FileItemUtc ?? DateTime.MinValue;
        }

        public DateTime GetDeletedFileItemLastUpdate()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Service is not initialized");

            return _lastUpdates?.DeletedFileItemUtc ?? DateTime.MinValue;
        }

        public DateTime GetTranscribeItemLastUpdate()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Service is not initialized");

            return _lastUpdates?.TranscribeItemUtc ?? DateTime.MinValue;
        }

        public DateTime GetUserSubscriptionLastUpdate()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Service is not initialized");

            return _lastUpdates?.UserSubscriptionUtc ?? DateTime.MinValue;
        }

        public DateTime GetInformationMessageLastUpdate()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Service is not initialized");

            return _lastUpdates?.InformationMessageUtc ?? DateTime.MinValue;
        }
    }
}
