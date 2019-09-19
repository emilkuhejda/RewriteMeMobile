using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface ITranscriptAudioSourceRepository
    {
        Task<TranscriptAudioSource> GetAsync(Guid transcribeItemId);

        Task InsertOrUpdateAsync(TranscriptAudioSource transcriptAudioSource);
    }
}
