﻿using System;
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
                DateCreated = new DateTimeOffset(entity.DateCreated, entity.DateCreatedOffset),
                AudioFiles = entity.AudioFiles?.Select(x => x.ToRecordedAudioFile())
            };
        }

        public static RecordedItemEntity ToRecordedItemEntity(this RecordedItem recordedItem)
        {
            return new RecordedItemEntity
            {
                Id = recordedItem.Id,
                DateCreated = recordedItem.DateCreated.DateTime,
                DateCreatedOffset = recordedItem.DateCreated.Offset,
                AudioFiles = recordedItem.AudioFiles?.Select(x => x.ToRecordedAudioFileEntity()).ToArray()
            };
        }
    }
}
