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

        public string Transcript { get; set; }

        public string UserTranscript { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public TimeSpan TotalTime { get; set; }

        public byte[] Source { get; set; }

        public string RecognitionSpeechResult { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
