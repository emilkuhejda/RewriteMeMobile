using System;

namespace RewriteMe.Domain.Transcription
{
    public class TranscriptAudioSource
    {
        public Guid Id { get; set; }

        public Guid TranscribeItemId { get; set; }

        public byte[] Source { get; set; }
    }
}
