using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace RewriteMe.DataAccess.Entities
{
    [Table("TranscribeItem")]
    public class TranscribeItemEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [ForeignKey(typeof(FileItemEntity))]
        public Guid FileItemId { get; set; }

        public string Alternatives { get; set; }

        public string UserTranscript { get; set; }

        public byte[] Source { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public TimeSpan TotalTime { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public bool IsPendingSynchronization { get; set; }
    }
}
