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
                RecognitionState = entity.RecognitionState,
                DateCreated = entity.DateCreated,
                DateProcessed = entity.DateProcessed,
                Version = entity.Version
            };
        }

        public static FileItemEntity ToFileItemEntity(this FileItem fileItem)
        {
            return new FileItemEntity
            {
                Id = fileItem.Id.GetValueOrDefault(),
                Name = fileItem.Name,
                FileName = fileItem.FileName,
                Language = fileItem.Language,
                RecognitionState = fileItem.RecognitionState,
                DateCreated = fileItem.DateCreated.GetValueOrDefault(),
                DateProcessed = fileItem.DateProcessed.GetValueOrDefault(),
                Version = fileItem.Version.GetValueOrDefault()
            };
        }
    }
}
