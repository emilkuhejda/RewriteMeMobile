using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace RewriteMe.DataAccess.Entities
{
    public class RecordedItemEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public bool IsRecordingOnly { get; set; }

        [MaxLength(100)]
        public string FileName { get; set; }

        public DateTime DateCreated { get; set; }

        public TimeSpan DateCreatedOffset { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public RecordedAudioFileEntity[] AudioFiles { get; set; }
    }
}
