using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IFileItemRepository _fileItemRepository;
        private readonly IDeletedFileItemRepository _deletedFileItemRepository;
        private readonly ILogger _logger;

        public UserSubscriptionService(
            IInternalValueService internalValueService,
            IRewriteMeWebService rewriteMeWebService,
            IUserSubscriptionRepository userSubscriptionRepository,
            IFileItemRepository fileItemRepository,
            IDeletedFileItemRepository deletedFileItemRepository,
            ILoggerFactory loggerFactory)
        {
            _internalValueService = internalValueService;
            _rewriteMeWebService = rewriteMeWebService;
            _userSubscriptionRepository = userSubscriptionRepository;
            _fileItemRepository = fileItemRepository;
            _deletedFileItemRepository = deletedFileItemRepository;
            _logger = loggerFactory.CreateLogger(typeof(UserSubscriptionService));
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastUserSubscriptionSynchronization = await _internalValueService.GetValueAsync(InternalValues.UserSubscriptionSynchronization).ConfigureAwait(false);
            _logger.Debug($"Update user subscriptions with timestamp '{lastUserSubscriptionSynchronization.ToString("d", CultureInfo.InvariantCulture)}'.");

            if (applicationUpdateDate >= lastUserSubscriptionSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetUserSubscriptionsAsync(lastUserSubscriptionSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    var userSubscriptions = httpRequestResult.Payload.ToList();
                    _logger.Info($"Web server returned {userSubscriptions.Count} items for synchronization.");

                    await _userSubscriptionRepository.InsertOrReplaceAllAsync(userSubscriptions).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.UserSubscriptionSynchronization, DateTime.UtcNow).ConfigureAwait(false);
                }
            }
        }

        public async Task RecognizedTimeSynchronizationAsync()
        {
            var recognizedTimeTicks = await _internalValueService.GetValueAsync(InternalValues.RecognizedTimeTicks).ConfigureAwait(false);
            if (recognizedTimeTicks > 0)
                return;

            var httpRequestResult = await _rewriteMeWebService.GetRecognizedTimeAsync().ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                await _internalValueService
                    .UpdateValueAsync(InternalValues.RecognizedTimeTicks, httpRequestResult.Payload.TotalTime.Ticks)
                    .ConfigureAwait(false);
            }
        }

        public async Task AddAsync(UserSubscription userSubscription)
        {
            await _userSubscriptionRepository.AddAsync(userSubscription).ConfigureAwait(false);
        }

        public async Task<TimeSpan> GetRemainingTimeAsync()
        {
            var deletedFilesTotalTime = await _internalValueService.GetValueAsync(InternalValues.DeletedFileItemsTotalTime).ConfigureAwait(false);
            var recognizedTimeTicks = await _internalValueService.GetValueAsync(InternalValues.RecognizedTimeTicks).ConfigureAwait(false);

            var userSubscriptionsTime = await _userSubscriptionRepository.GetTotalTimeAsync().ConfigureAwait(false);
            var processedFilesTotalTime = await _fileItemRepository.GetProcessedFilesTotalTimeAsync().ConfigureAwait(false);
            var processedDeletedFilesTotalTime = await _deletedFileItemRepository.GetProcessedFilesTotalTimeAsync().ConfigureAwait(false);

            processedFilesTotalTime = processedFilesTotalTime
                .Add(processedDeletedFilesTotalTime)
                .Add(TimeSpanHelper.Parse(deletedFilesTotalTime))
                .Add(TimeSpan.FromTicks(recognizedTimeTicks));
            return userSubscriptionsTime.Subtract(processedFilesTotalTime);
        }
    }
}
