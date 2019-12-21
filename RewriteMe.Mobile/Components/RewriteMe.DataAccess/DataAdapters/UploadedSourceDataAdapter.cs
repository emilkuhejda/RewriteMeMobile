using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.Upload;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class UploadedSourceDataAdapter
    {
        public static UploadedSource ToUploadedSource(this UploadedSourceEntity entity)
        {
            return new UploadedSource
            {
                Id = entity.Id,
                FileItemId = entity.FileItemId,
                Name = entity.Name,
                FileName = entity.FileName,
                Language = entity.Language,
                Source = entity.Source,
                IsTranscript = entity.IsTranscript,
                DateCreated = entity.DateCreated
            };
        }

        public static UploadedSourceEntity ToUploadedSourceEntity(this UploadedSource uploadedSource)
        {
            return new UploadedSourceEntity
            {
                Id = uploadedSource.Id,
                FileItemId = uploadedSource.FileItemId,
                Name = uploadedSource.Name,
                FileName = uploadedSource.FileName,
                Language = uploadedSource.Language,
                Source = uploadedSource.Source,
                IsTranscript = uploadedSource.IsTranscript,
                DateCreated = uploadedSource.DateCreated
            };
        }
    }
}
