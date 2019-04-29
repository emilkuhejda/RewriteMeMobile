using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace RewriteMe.DataAccess.Entities
{
    [Table("AudioSource")]
    public class AudioSourceEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [ForeignKey(typeof(FileItemEntity))]
        public Guid FileItemId { get; set; }

        public string ContentType { get; set; }

        public int Version { get; set; }
    }
}
