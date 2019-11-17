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
        private readonly IDeletedFileItemRepository _deletedFileItemRepository;
        private readonly IInformationMessageRepository _informationMessageRepository;

        public CleanUpService(
            IInternalValueService internalValueService,
            IUserSessionRepository userSessionRepository,
            IFileItemRepository fileItemRepository,
            IDeletedFileItemRepository deletedFileItemRepository,
            IInformationMessageRepository informationMessageRepository)
        {
            _internalValueService = internalValueService;
            _userSessionRepository = userSessionRepository;
            _fileItemRepository = fileItemRepository;
            _deletedFileItemRepository = deletedFileItemRepository;
            _informationMessageRepository = informationMessageRepository;
        }

        public async Task CleanUp()
        {
            await _internalValueService.UpdateValueAsync(InternalValues.FileItemSynchronizationTicks, 0).ConfigureAwait(false);
            await _internalValueService.UpdateValueAsync(InternalValues.TranscribeItemSynchronizationTicks, 0).ConfigureAwait(false);
            await _internalValueService.UpdateValueAsync(InternalValues.UserSubscriptionSynchronizationTicks, 0).ConfigureAwait(false);
            await _internalValueService.UpdateValueAsync(InternalValues.InformationMessageSynchronizationTicks, 0).ConfigureAwait(false);
            await _internalValueService.UpdateValueAsync(InternalValues.RemainingTimeTicks, 0).ConfigureAwait(false);
            await _internalValueService.UpdateValueAsync(InternalValues.ApplicationId, null).ConfigureAwait(false);

            await _fileItemRepository.ClearAsync().ConfigureAwait(false);
            await _deletedFileItemRepository.ClearAsync().ConfigureAwait(false);
            await _userSessionRepository.ClearAsync().ConfigureAwait(false);
            await _informationMessageRepository.ClearAsync().ConfigureAwait(false);
        }
    }
}
