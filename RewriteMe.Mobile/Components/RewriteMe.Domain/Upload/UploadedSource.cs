﻿using System;

namespace RewriteMe.Domain.Upload
{
    public class UploadedSource
    {
        public Guid Id { get; set; }

        public Guid FileItemId { get; set; }

        public string Language { get; set; }

        public bool IsPhoneCall { get; set; }

        public bool IsTimeFrame { get; set; }

        public TimeSpan TranscriptionStartTime { get; set; }

        public TimeSpan TranscriptionEndTime { get; set; }

        public byte[] Source { get; set; }

        public bool IsTranscript { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
