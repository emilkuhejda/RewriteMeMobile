using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace RewriteMe.DataAccess.Entities
{
    public class RecordedItemEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [MaxLength(150)]
        public string FileName { get; set; }

        [MaxLength(250)]
        public string Path { get; set; }

        public DateTime DateCreated { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public RecordedAudioFileEntity[] AudioFiles { get; set; }
    }
}
