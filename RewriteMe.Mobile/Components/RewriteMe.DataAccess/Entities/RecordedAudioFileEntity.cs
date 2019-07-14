using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace RewriteMe.DataAccess.Entities
{
    public class RecordedAudioFileEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [ForeignKey(typeof(RecordedItemEntity))]
        public Guid RecordedItemId { get; set; }

        [MaxLength(250)]
        public string Path { get; set; }

        public string Transcript { get; set; }

        public string UserTranscript { get; set; }

        public string RecognitionSpeechResult { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
