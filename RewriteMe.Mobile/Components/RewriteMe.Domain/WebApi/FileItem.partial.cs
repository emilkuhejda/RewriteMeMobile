using System;
using RewriteMe.Common.Utils;

namespace RewriteMe.Domain.WebApi
{
    public partial class FileItem
    {
        public RecognitionState RecognitionState => EnumHelper.Parse(RecognitionStateString, RecognitionState.None);

        public TimeSpan TotalTime => TimeSpan.FromTicks(TotalTimeTicks);

        public bool IsTimeFrame => TranscriptionStartTime != TimeSpan.Zero || TranscriptionEndTime != TimeSpan.Zero;

        public TimeSpan TranscriptionStartTime => TimeSpan.FromTicks(TranscriptionStartTimeTicks);

        public TimeSpan TranscriptionEndTime => TimeSpan.FromTicks(TranscriptionEndTimeTicks);

        public TimeSpan TranscribedTime => TimeSpan.FromTicks(TranscribedTimeTicks);

        public ErrorCode? UploadErrorCode { get; set; }

        public ErrorCode? TranscribeErrorCode { get; set; }
    }
}
