using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class CleanUpService : ICleanUpService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IFileItemRepository _fileItemRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly ISubscriptionProductRepository _subscriptionProductRepository;

        public CleanUpService(
            IInternalValueService internalValueService,
            IUserSessionRepository userSessionRepository,
            IFileItemRepository fileItemRepository,
            IUserSubscriptionRepository userSubscriptionRepository,
            ISubscriptionProductRepository subscriptionProductRepository)
        {
            _internalValueService = internalValueService;
            _userSessionRepository = userSessionRepository;
            _fileItemRepository = fileItemRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _subscriptionProductRepository = subscriptionProductRepository;
        }

        public async Task CleanUp()
        {
            await _internalValueService.UpdateValueAsync(InternalValues.FileItemSynchronization, DateTime.MinValue).ConfigureAwait(false);
            await _internalValueService.UpdateValueAsync(InternalValues.TranscribeItemSynchronization, DateTime.MinValue).ConfigureAwait(false);
            await _internalValueService.UpdateValueAsync(InternalValues.UserSubscriptionSynchronization, DateTime.MinValue).ConfigureAwait(false);
            await _internalValueService.UpdateValueAsync(InternalValues.SubscriptionProductSynchronization, DateTime.MinValue).ConfigureAwait(false);
            await _internalValueService.UpdateValueAsync(InternalValues.ApplicationId, null).ConfigureAwait(false);

            await _fileItemRepository.ClearAsync().ConfigureAwait(false);
            await _userSessionRepository.ClearAsync().ConfigureAwait(false);
            await _userSubscriptionRepository.ClearAsync().ConfigureAwait(false);
            await _subscriptionProductRepository.ClearAsync().ConfigureAwait(false);
        }
    }
}
