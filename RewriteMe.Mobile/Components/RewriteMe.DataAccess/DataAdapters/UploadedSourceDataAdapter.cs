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
                Source = entity.Source,
                DateCreated = entity.DateCreated
            };
        }

        public static UploadedSourceEntity ToUploadedSourceEntity(this UploadedSource uploadedSource)
        {
            return new UploadedSourceEntity
            {
                Id = uploadedSource.Id,
                FileItemId = uploadedSource.FileItemId,
                Source = uploadedSource.Source,
                DateCreated = uploadedSource.DateCreated
            };
        }
    }
}
