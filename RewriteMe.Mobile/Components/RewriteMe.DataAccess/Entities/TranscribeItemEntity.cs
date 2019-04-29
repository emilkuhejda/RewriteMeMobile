using System;
using SQLite;

namespace RewriteMe.DataAccess.Entities
{
    public class TranscribeItemEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public Guid FileItemId { get; set; }

        public string Alternatives { get; set; }

        public string UserTranscript { get; set; }

        public byte[] Source { get; set; }

        [MaxLength(100)]
        public string StartTime { get; set; }

        [MaxLength(100)]
        public string EndTime { get; set; }

        [MaxLength(100)]
        public string TotalTime { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}
