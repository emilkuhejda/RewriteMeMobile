﻿using System;
using System.Collections.Generic;

namespace RewriteMe.Domain.Transcription
{
    public class RecordedItem
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string Path { get; set; }

        public string UserTranscript { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        public IEnumerable<RecordedAudioFile> AudioFiles { get; set; }
    }
}