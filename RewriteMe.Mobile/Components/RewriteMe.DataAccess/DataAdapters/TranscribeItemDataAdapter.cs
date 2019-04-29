using System.Collections.Generic;
using Newtonsoft.Json;
using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.WebApi.Models;

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
                Source = entity.Source,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                TotalTime = entity.TotalTime,
                DateCreated = entity.DateCreated,
                Version = entity.Version
            };
        }

        public static TranscribeItemEntity ToTranscribeItemEntity(this TranscribeItem transcribeItem)
        {
            return new TranscribeItemEntity
            {
                Id = transcribeItem.Id.GetValueOrDefault(),
                FileItemId = transcribeItem.FileItemId.GetValueOrDefault(),
                Alternatives = JsonConvert.SerializeObject(transcribeItem.Alternatives),
                UserTranscript = transcribeItem.UserTranscript,
                Source = transcribeItem.Source,
                StartTime = transcribeItem.StartTime,
                EndTime = transcribeItem.EndTime,
                TotalTime = transcribeItem.TotalTime,
                DateCreated = transcribeItem.DateCreated.GetValueOrDefault(),
                Version = transcribeItem.Version.GetValueOrDefault()
            };
        }
    }
}
