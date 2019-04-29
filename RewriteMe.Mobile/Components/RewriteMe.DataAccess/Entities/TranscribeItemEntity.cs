using System;

namespace RewriteMe.DataAccess.Entities
{
    public class TranscribeItemEntity
    {
        public Guid Id { get; set; }

        public Guid FileItemId { get; set; }

        public string Alternatives { get; set; }

        public string UserTranscript { get; set; }

        public byte[] Source { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string TotalTime { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}
