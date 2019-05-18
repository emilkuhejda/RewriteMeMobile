using System;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Configuration
{
    public class DeletedFileItem
    {
        public Guid Id { get; set; }

        public DateTime DeletedDate { get; set; }

        public RecognitionState RecognitionState { get; set; }

        public TimeSpan TotalTime { get; set; }
    }
}
