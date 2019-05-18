using System;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class DeletedFileItem
    {
        public RecognitionState RecognitionState { get; set; }

        public TimeSpan TotalTime { get; set; }
    }
}
