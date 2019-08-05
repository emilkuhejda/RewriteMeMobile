﻿using System;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class FileItem
    {
        public RecognitionState RecognitionState => EnumHelper.Parse(RecognitionStateString, RecognitionState.None);

        public TimeSpan TotalTime => new TimeSpan(TotalTimeTicks);

        public TimeSpan TranscribedTime => new TimeSpan(TranscribedTimeTicks);
    }
}
