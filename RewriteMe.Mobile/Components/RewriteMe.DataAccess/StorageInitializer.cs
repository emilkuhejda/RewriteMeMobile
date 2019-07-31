using System.Threading.Tasks;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.DataAccess
{
    public class StorageInitializer : IStorageInitializer
    {
        private readonly IRecordedItemService _recordedItemService;
        private readonly IAppDbContextProvider _contextProvider;

        public StorageInitializer(
            IRecordedItemService recordedItemService,
            IAppDbContextProvider contextProvider)
        {
            _recordedItemService = recordedItemService;
            _contextProvider = contextProvider;
        }

        public async Task InitializeAsync()
        {
            _recordedItemService.CreateDirectory();

            var tables = new[]
            {
                typeof(DeletedFileItemEntity),
                typeof(FileItemEntity),
                typeof(InternalValueEntity),
                typeof(RecordedAudioFileEntity),
                typeof(RecordedItemEntity),
                typeof(TranscribeItemEntity),
                typeof(TranscriptAudioSourceEntity),
                typeof(UserSessionEntity),
                typeof(UserSubscriptionEntity)
            };

            await _contextProvider.Context.CreateTablesAsync(tables).ConfigureAwait(false);
        }
    }
}
