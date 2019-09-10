using System;
using SQLite;

namespace RewriteMe.DataAccess.Entities
{
    [Table("TranscriptAudioSource")]
    public class TranscriptAudioSourceEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public Guid TranscribeItemId { get; set; }

        public byte[] Source { get; set; }
    }
}
