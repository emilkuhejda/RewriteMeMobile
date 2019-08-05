using System;
using RewriteMe.Domain.Transcription;
using SQLite;

namespace RewriteMe.DataAccess.Entities
{
    [Table("DeletedFileItem")]
    public class DeletedFileItemEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public DateTime DeletedDate { get; set; }

        public RecognitionState RecognitionState { get; set; }

        public TimeSpan TotalTime { get; set; }

        public TimeSpan TranscribedTime { get; set; }
    }
}
