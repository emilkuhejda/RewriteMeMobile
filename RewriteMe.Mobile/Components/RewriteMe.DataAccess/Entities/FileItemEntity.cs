using System;
using RewriteMe.Domain.WebApi;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace RewriteMe.DataAccess.Entities
{
    [Table("FileItem")]
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

        [MaxLength(20)]
        public RecognitionState RecognitionState { get; set; }

        public TimeSpan TotalTime { get; set; }

        public TimeSpan TranscribedTime { get; set; }

        public UploadStatus UploadStatus { get; set; }

        public ErrorCode? UploadErrorCode { get; set; }

        public ErrorCode? TranscribeErrorCode { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateProcessedUtc { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public TranscribeItemEntity[] TranscribeItems { get; set; }
    }
}
