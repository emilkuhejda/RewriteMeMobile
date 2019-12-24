using System;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.WebApi
{
    public partial class FileItem
    {
        public RecognitionState RecognitionState => EnumHelper.Parse(RecognitionStateString, RecognitionState.None);

        public TimeSpan TotalTime => TimeSpan.FromTicks(TotalTimeTicks);

        public TimeSpan TranscribedTime => TimeSpan.FromTicks(TranscribedTimeTicks);

        public UploadStatus UploadStatus { get; set; }

        public bool IsUploading => UploadStatus == UploadStatus.InProgress;

        public int? UploadErrorCode { get; set; }

        public int? TranscribeErrorCode { get; set; }
    }
}
