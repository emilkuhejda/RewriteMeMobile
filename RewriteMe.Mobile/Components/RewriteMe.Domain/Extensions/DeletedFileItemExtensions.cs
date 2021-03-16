using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Extensions
{
    public static class DeletedFileItemExtensions
    {
        public static DeletedAudioFileInputModel ToDeletedFileItemModel(this DeletedFileItem deletedFileItem)
        {
            return new DeletedAudioFileInputModel
            {
                Id = deletedFileItem.Id,
                DeletedDate = deletedFileItem.DeletedDate
            };
        }
    }
}
