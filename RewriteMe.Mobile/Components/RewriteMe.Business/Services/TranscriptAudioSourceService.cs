using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Business.Services
{
    public class TranscriptAudioSourceService : ITranscriptAudioSourceService
    {
        private readonly ITranscriptAudioSourceRepository _transcriptAudioSourceRepository;

        public TranscriptAudioSourceService(ITranscriptAudioSourceRepository transcriptAudioSourceRepository)
        {
            _transcriptAudioSourceRepository = transcriptAudioSourceRepository;
        }

        public async Task<TranscriptAudioSource> GetAsync(Guid transcribeItemId)
        {
            return await _transcriptAudioSourceRepository.GetAsync(transcribeItemId).ConfigureAwait(false);
        }

        public async Task InsertAsync(TranscriptAudioSource transcriptAudioSource)
        {
            await _transcriptAudioSourceRepository.InsertAsync(transcriptAudioSource).ConfigureAwait(false);
        }
    }
}
