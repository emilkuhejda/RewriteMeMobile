using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Business.Services
{
    public class RecordedItemService : IRecordedItemService
    {
        private readonly IRecordedItemRepository _recordedItemRepository;

        public RecordedItemService(IRecordedItemRepository recordedItemRepository)
        {
            _recordedItemRepository = recordedItemRepository;
        }

        public async Task<IEnumerable<RecordedItem>> GetAllAsync()
        {
            return await _recordedItemRepository.GetAllAsync().ConfigureAwait(false);
        }
    }
}
