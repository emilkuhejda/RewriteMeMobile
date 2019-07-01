using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class RecordedAudioFileDataAdapter
    {
        public static RecordedAudioFile ToRecordedAudioFile(this RecordedAudioFileEntity entity)
        {
            return new RecordedAudioFile
            {
                Id = entity.Id,
                RecordedItemId = entity.RecordedItemId,
                Path = entity.Path,
                Transcript = entity.Transcript,
                DateCreated = entity.DateCreated
            };
        }

        public static RecordedAudioFileEntity ToRecordedAudioFileEntity(this RecordedAudioFile recordedAudioFile)
        {
            return new RecordedAudioFileEntity
            {
                Id = recordedAudioFile.Id,
                RecordedItemId = recordedAudioFile.RecordedItemId,
                Path = recordedAudioFile.Path,
                Transcript = recordedAudioFile.Path,
                DateCreated = recordedAudioFile.DateCreated
            };
        }
    }
}
