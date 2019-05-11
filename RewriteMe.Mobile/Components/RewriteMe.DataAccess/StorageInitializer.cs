using System.Threading.Tasks;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;

namespace RewriteMe.DataAccess
{
    public class StorageInitializer : IStorageInitializer
    {
        private readonly IAppDbContextProvider _contextProvider;

        public StorageInitializer(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task InitializeAsync()
        {
            var tables = new[]
            {
                typeof(AudioSourceEntity),
                typeof(FileItemEntity),
                typeof(InternalValueEntity),
                typeof(TranscribeItemEntity),
                typeof(TranscriptAudioSourceEntity),
                typeof(UserSessionEntity),
                typeof(UserSubscriptionEntity)
            };

            await _contextProvider.Context.CreateTablesAsync(tables).ConfigureAwait(false);
        }
    }
}
