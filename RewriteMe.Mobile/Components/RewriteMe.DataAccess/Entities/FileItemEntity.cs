using System;
using SQLite;

namespace RewriteMe.DataAccess.Entities
{
    public class FileItemEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(150)]
        public string FileName { get; set; }

        [MaxLength(20)]
        public string Language { get; set; }

        [MaxLength(50)]
        public string RecognitionState { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateProcessed { get; set; }

        public DateTime DateUpdated { get; set; }

        public int? AudioSourceVersion { get; set; }
    }
}
