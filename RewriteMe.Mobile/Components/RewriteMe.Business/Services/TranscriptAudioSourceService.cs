﻿using System;
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

        public async Task<bool> SynchronizeAsync(Guid transcribeItemId, CancellationToken cancellationToken)
        {
            return await RefreshAsync(Guid.NewGuid(), transcribeItemId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> RefreshAsync(Guid audioSourceId, Guid transcribeItemId, CancellationToken cancellationToken)
        {
            var httpRequestResult = await _rewriteMeWebService.GetTranscribeAudioSourceAsync(transcribeItemId, cancellationToken).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Offline || httpRequestResult.State == HttpRequestState.Canceled)
                return false;

            if (httpRequestResult.State == HttpRequestState.Error && httpRequestResult.StatusCode.HasValue && httpRequestResult.StatusCode != 404)
                return false;

            var source = httpRequestResult.Payload ?? Array.Empty<byte>();
            var audioSource = new TranscriptAudioSource
            {
                Id = audioSourceId,
                TranscribeItemId = transcribeItemId,
                Source = source
            };

            await _transcriptAudioSourceRepository.InsertOrUpdateAsync(audioSource).ConfigureAwait(false);
            return true;
        }

        public async Task<TranscriptAudioSource> GetAsync(Guid transcribeItemId)
        {
            return await _transcriptAudioSourceRepository.GetAsync(transcribeItemId).ConfigureAwait(false);
        }
    }
}
