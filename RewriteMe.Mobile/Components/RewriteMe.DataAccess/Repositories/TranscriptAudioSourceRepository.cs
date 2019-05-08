using System;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.DataAccess.Repositories
{
    public class TranscriptAudioSourceRepository : ITranscriptAudioSourceRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public TranscriptAudioSourceRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<TranscriptAudioSource> GetAsync(Guid transcribeItemId)
        {
            var entity = await _contextProvider.Context.FindAsync<TranscriptAudioSourceEntity>(x => x.TranscribeItemId == transcribeItemId).ConfigureAwait(false);

            return entity?.ToTranscriptAudioSource();
        }

        public async Task InsertAsync(TranscriptAudioSource transcriptAudioSource)
        {
            await _contextProvider.Context.InsertAsync(transcriptAudioSource.ToTranscriptAudioSourceEntity()).ConfigureAwait(false);
        }
    }
}
