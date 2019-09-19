using System;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ITranscriptAudioSourceService
    {
        Task<bool> RefreshAsync(Guid audioSourceId, Guid transcribeItemId, CancellationToken cancellationToken);

        Task<bool> SynchronizeAsync(Guid transcribeItemId, CancellationToken cancellationToken);

        Task<TranscriptAudioSource> GetAsync(Guid transcribeItemId);
    }
}
