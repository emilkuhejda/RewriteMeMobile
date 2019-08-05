using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.WebApi.Models;

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
                DateProcessed = entity.DateProcessed,
                DateUpdated = entity.DateUpdated
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
                DateProcessed = fileItem.DateProcessed,
                DateUpdated = fileItem.DateUpdated
            };
        }
    }
}
