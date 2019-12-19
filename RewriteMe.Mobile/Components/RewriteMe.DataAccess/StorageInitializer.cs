using System.Threading.Tasks;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.DataAccess
{
    public class StorageInitializer : IStorageInitializer
    {
        private const int DatabaseVersionNumber = 0;

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
                typeof(InformationMessageEntity),
                typeof(InternalValueEntity),
                typeof(LanguageVersionEntity),
                typeof(RecordedAudioFileEntity),
                typeof(RecordedItemEntity),
                typeof(TranscribeItemEntity),
                typeof(TranscriptAudioSourceEntity),
                typeof(UploadedSourceEntity),
                typeof(UserSessionEntity)
            };

            var versionNumber = await _contextProvider.Context.GetVersionNumberAsync().ConfigureAwait(false);
            if (versionNumber < DatabaseVersionNumber)
            {
                foreach (var table in tables)
                {
                    await _contextProvider.Context.DropTable(table).ConfigureAwait(false);
                }

                await _contextProvider.Context.UpdateVersionNumberAsync(DatabaseVersionNumber).ConfigureAwait(false);
            }

            await _contextProvider.Context.CreateTablesAsync(tables).ConfigureAwait(false);
        }
    }
}
