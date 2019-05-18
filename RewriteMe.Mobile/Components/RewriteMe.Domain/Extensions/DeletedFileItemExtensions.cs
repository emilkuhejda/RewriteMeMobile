using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Extensions
{
    public static class DeletedFileItemExtensions
    {
        public static DeletedFileItemModel ToDeletedFileItemModel(this DeletedFileItem deletedFileItem)
        {
            return new DeletedFileItemModel
            {
                Id = deletedFileItem.Id,
                DeletedDate = deletedFileItem.DeletedDate
            };
        }
    }
}
