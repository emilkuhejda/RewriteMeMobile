using System.Linq;
using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class RecordedItemDataAdapter
    {
        public static RecordedItem ToRecordedItem(this RecordedItemEntity entity)
        {
            return new RecordedItem
            {
                Id = entity.Id,
                FileName = entity.FileName,
                Path = entity.Path,
                DateCreated = entity.DateCreated,
                AudioFiles = entity.AudioFiles?.Select(x => x.ToRecordedAudioFile())
            };
        }

        public static RecordedItemEntity ToRecordedItemEntity(this RecordedItem recordedItem)
        {
            return new RecordedItemEntity
            {
                Id = recordedItem.Id,
                FileName = recordedItem.FileName,
                Path = recordedItem.Path,
                DateCreated = recordedItem.DateCreated,
                AudioFiles = recordedItem.AudioFiles?.Select(x => x.ToRecordedAudioFileEntity()).ToArray()
            };
        }
    }
}
