using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Services
{
    public class LastUpdatesService : ILastUpdatesService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IApplicationSettings _applicationSettings;

        private LastUpdates _lastUpdates;
        private bool _isInitialized;

        public LastUpdatesService(
            IInternalValueService internalValueService,
            IRewriteMeWebService rewriteMeWebService,
            IApplicationSettings applicationSettings)
        {
            _internalValueService = internalValueService;
            _rewriteMeWebService = rewriteMeWebService;
            _applicationSettings = applicationSettings;
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

        public async Task InitializeApplicationSettingsAsync()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("Service is not initialized");

            var apiUrl = await _internalValueService.GetValueAsync(InternalValues.ApiUrl).ConfigureAwait(false);
            if (!_lastUpdates.ApiUrl.Equals(apiUrl, StringComparison.OrdinalIgnoreCase))
            {
                await _internalValueService.UpdateValueAsync(InternalValues.ApiUrl, _lastUpdates.ApiUrl).ConfigureAwait(false);
            }

            var isApplicationOutOfDate = false;
            if (int.TryParse(_applicationSettings.WebApiVersion, out var webApiVersion))
            {
                if (_lastUpdates.ApiVersion.Major > webApiVersion)
                {
                    isApplicationOutOfDate = true;
                }
            }

            await _internalValueService.UpdateValueAsync(InternalValues.IsApplicationOutOfDate, isApplicationOutOfDate).ConfigureAwait(false);
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
