using System;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Business.Services
{
    public class TranscriptAudioSourceService : ITranscriptAudioSourceService
    {
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly ITranscriptAudioSourceRepository _transcriptAudioSourceRepository;

        public TranscriptAudioSourceService(
            IRewriteMeWebService rewriteMeWebService,
            ITranscriptAudioSourceRepository transcriptAudioSourceRepository)
        {
            _rewriteMeWebService = rewriteMeWebService;
            _transcriptAudioSourceRepository = transcriptAudioSourceRepository;
        }

        public async Task SynchronizeAsync(Guid transcribeItemId, CancellationToken cancellationToken)
        {
            var httpRequestResult = await _rewriteMeWebService.GetTranscribeAudioSourceAsync(transcribeItemId, cancellationToken).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                var source = httpRequestResult.Payload;

                var audioSource = new TranscriptAudioSource
                {
                    Id = Guid.NewGuid(),
                    TranscribeItemId = transcribeItemId,
                    Source = source
                };

                await _transcriptAudioSourceRepository.InsertAsync(audioSource).ConfigureAwait(false);
            }
        }

        public async Task<TranscriptAudioSource> GetAsync(Guid transcribeItemId)
        {
            return await _transcriptAudioSourceRepository.GetAsync(transcribeItemId).ConfigureAwait(false);
        }
    }
}
