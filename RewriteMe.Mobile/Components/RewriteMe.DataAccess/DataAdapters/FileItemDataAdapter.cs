using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class FileItemDataAdapter
    {
        public static FileItem ToFileItem(this FileItemEntity entity)
        {
            return new FileItem
            {
                Id = entity.Id,
                Name = entity.Name,
                FileName = entity.FileName,
                Language = entity.Language,
                RecognitionStateString = entity.RecognitionState.ToString(),
                TotalTimeTicks = entity.TotalTime.Ticks,
                TranscribedTimeTicks = entity.TranscribedTime.Ticks,
                DateCreated = entity.DateCreated,
                DateProcessedUtc = entity.DateProcessedUtc,
                DateUpdatedUtc = entity.DateUpdatedUtc
            };
        }

        public static FileItemEntity ToFileItemEntity(this FileItem fileItem)
        {
            return new FileItemEntity
            {
                Id = fileItem.Id,
                Name = fileItem.Name,
                FileName = fileItem.FileName,
                Language = fileItem.Language,
                RecognitionState = fileItem.RecognitionState,
                TotalTime = fileItem.TotalTime,
                TranscribedTime = fileItem.TranscribedTime,
                DateCreated = fileItem.DateCreated,
                DateProcessedUtc = fileItem.DateProcessedUtc,
                DateUpdatedUtc = fileItem.DateUpdatedUtc
            };
        }
    }
}
