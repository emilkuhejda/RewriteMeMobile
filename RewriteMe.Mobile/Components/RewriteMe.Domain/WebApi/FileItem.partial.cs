using System;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.WebApi
{
    public partial class FileItem
    {
        public RecognitionState RecognitionState => EnumHelper.Parse(RecognitionStateString, RecognitionState.None);

        public TimeSpan TotalTime => TimeSpan.FromTicks(TotalTimeTicks);

        public TimeSpan TranscribedTime => TimeSpan.FromTicks(TranscribedTimeTicks);

        public ErrorCode? UploadErrorCode { get; set; }

        public ErrorCode? TranscribeErrorCode { get; set; }
    }
}
