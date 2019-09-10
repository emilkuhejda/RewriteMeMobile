using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace RewriteMe.DataAccess.Entities
{
    [Table("TranscriptAudioSource")]
    public class TranscriptAudioSourceEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [ForeignKey(typeof(TranscribeItemEntity))]
        public Guid TranscribeItemId { get; set; }

        public byte[] Source { get; set; }
    }
}
