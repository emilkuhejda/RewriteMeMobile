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

        [MaxLength(20)]
        public string Language { get; set; }

        public bool IsPhoneCall { get; set; }

        public byte[] Source { get; set; }

        public bool IsTranscript { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
