using System;
using SQLite;

namespace RewriteMe.DataAccess.Entities
{
    [Table("DeletedFileItem")]
    public class DeletedFileItemEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public DateTime DeletedDate { get; set; }
    }
}
