using System;
using SQLite;

namespace RewriteMe.DataAccess.Entities
{
    [Table("UploadedSource")]
    public class UploadedSourceEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public Guid FileItemId { get; set; }

        public byte[] Source { get; set; }

        public bool IsTranscript { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
