using System;
using System.Collections.Generic;

namespace RewriteMe.Domain.Transcription
{
    public class RecordedItem
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public string Path { get; set; }

        public DateTime DateCreated { get; set; }

        public IEnumerable<RecordedAudioFile> AudioFiles { get; set; }
    }
}
