using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class TranscriptAudioSourceDataAdapter
    {
        public static TranscriptAudioSource ToTranscriptAudioSource(this TranscriptAudioSourceEntity entity)
        {
            return new TranscriptAudioSource
            {
                Id = entity.Id,
                TranscribeItemId = entity.TranscribeItemId,
                Source = entity.Source
            };
        }

        public static TranscriptAudioSourceEntity ToTranscriptAudioSourceEntity(
            this TranscriptAudioSource transcriptAudioSource)
        {
            return new TranscriptAudioSourceEntity
            {
                Id = transcriptAudioSource.Id,
                TranscribeItemId = transcriptAudioSource.TranscribeItemId,
                Source = transcriptAudioSource.Source
            };
        }
    }
}
