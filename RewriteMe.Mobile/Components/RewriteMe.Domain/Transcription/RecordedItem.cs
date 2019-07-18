using System;
using System.Collections.Generic;

namespace RewriteMe.Domain.Transcription
{
    public class RecordedItem
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public bool IsRecordingOnly { get; set; }

        public string FileName { get; set; }

        public string AudioFileName => IsRecordingOnly ? $"{FileName}.wav" : string.Empty;

        public DateTimeOffset DateCreated { get; set; }

        public IEnumerable<RecordedAudioFile> AudioFiles { get; set; }
    }
}
