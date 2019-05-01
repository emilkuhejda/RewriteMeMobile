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

        public CleanUpService(
            IInternalValueService internalValueService,
            IUserSessionRepository userSessionRepository,
            IFileItemRepository fileItemRepository)
        {
            _internalValueService = internalValueService;
            _userSessionRepository = userSessionRepository;
            _fileItemRepository = fileItemRepository;
        }

        public async Task CleanUp()
        {
            await _internalValueService.UpdateValueAsync(InternalValues.FileItemSynchronization, DateTime.MinValue);
            await _internalValueService.UpdateValueAsync(InternalValues.TranscribeItemSynchronization, DateTime.MinValue);

            await _fileItemRepository.ClearAsync().ConfigureAwait(false);
            await _userSessionRepository.ClearAsync().ConfigureAwait(false);
        }
    }
}
