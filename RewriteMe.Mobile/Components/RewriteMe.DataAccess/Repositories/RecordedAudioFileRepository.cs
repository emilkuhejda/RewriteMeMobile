using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.DataAccess.Repositories
{
    public class RecordedAudioFileRepository : IRecordedAudioFileRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public RecordedAudioFileRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task InsertAsync(RecordedAudioFile recordedItem)
        {
            await _contextProvider.Context.InsertAsync(recordedItem.ToRecordedAudioFileEntity()).ConfigureAwait(false);
        }
    }
}
