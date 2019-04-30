using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Business.Services
{
    public class CleanUpService : ICleanUpService
    {
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IFileItemRepository _fileItemRepository;

        public CleanUpService(
            IUserSessionRepository userSessionRepository,
            IFileItemRepository fileItemRepository)
        {
            _userSessionRepository = userSessionRepository;
            _fileItemRepository = fileItemRepository;
        }

        public async Task CleanUp()
        {
            await _fileItemRepository.ClearAsync().ConfigureAwait(false);
            await _userSessionRepository.ClearAsync().ConfigureAwait(false);
        }
    }
}
