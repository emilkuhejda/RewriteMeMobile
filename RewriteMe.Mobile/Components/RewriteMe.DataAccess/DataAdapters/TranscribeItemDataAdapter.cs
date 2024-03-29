﻿using System.Collections.Generic;
using Newtonsoft.Json;
using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class TranscribeItemDataAdapter
    {
        public static TranscribeItem ToTranscribeItem(this TranscribeItemEntity entity)
        {
            return new TranscribeItem
            {
                Id = entity.Id,
                FileItemId = entity.FileItemId,
                Alternatives = JsonConvert.DeserializeObject<IList<RecognitionAlternative>>(entity.Alternatives),
                UserTranscript = entity.UserTranscript,
                StartTimeTicks = entity.StartTime.Ticks,
                EndTimeTicks = entity.EndTime.Ticks,
                TotalTimeTicks = entity.TotalTime.Ticks,
                DateCreatedUtc = entity.DateCreatedUtc,
                DateUpdatedUtc = entity.DateUpdatedUtc,
                IsPendingSynchronization = entity.IsPendingSynchronization
            };
        }

        public static TranscribeItemEntity ToTranscribeItemEntity(this TranscribeItem transcribeItem)
        {
            return new TranscribeItemEntity
            {
                Id = transcribeItem.Id,
                FileItemId = transcribeItem.FileItemId,
                Alternatives = JsonConvert.SerializeObject(transcribeItem.Alternatives),
                UserTranscript = transcribeItem.UserTranscript,
                StartTime = transcribeItem.StartTime,
                EndTime = transcribeItem.EndTime,
                TotalTime = transcribeItem.TotalTime,
                DateCreatedUtc = transcribeItem.DateCreatedUtc,
                DateUpdatedUtc = transcribeItem.DateCreatedUtc,
                IsPendingSynchronization = transcribeItem.IsPendingSynchronization
            };
        }
    }
}
