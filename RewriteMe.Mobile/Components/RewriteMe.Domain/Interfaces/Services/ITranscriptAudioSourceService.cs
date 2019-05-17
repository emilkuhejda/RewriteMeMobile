﻿using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ITranscriptAudioSourceService
    {
        Task<TranscriptAudioSource> GetAsync(Guid transcribeItemId);

        Task InsertAsync(TranscriptAudioSource transcriptAudioSource);
    }
}