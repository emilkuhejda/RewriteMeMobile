using System;

namespace RewriteMe.Domain.Configuration
{
    public class DeletedFileItem
    {
        public Guid Id { get; set; }

        public DateTime DeletedDate { get; set; }
    }
}
