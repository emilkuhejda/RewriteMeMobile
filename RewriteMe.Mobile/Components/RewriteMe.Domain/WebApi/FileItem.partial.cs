using System;
using RewriteMe.Common.Utils;

namespace RewriteMe.Domain.WebApi
{
    public partial class FileItem
    {
        public RecognitionState RecognitionState => EnumHelper.Parse(RecognitionStateString, RecognitionState.None);

        public bool IsTimeFrame { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public TimeSpan TotalTime => TimeSpan.FromTicks(TotalTimeTicks);

        public TimeSpan TranscribedTime => TimeSpan.FromTicks(TranscribedTimeTicks);

        public ErrorCode? UploadErrorCode { get; set; }

        public ErrorCode? TranscribeErrorCode { get; set; }
    }
}
