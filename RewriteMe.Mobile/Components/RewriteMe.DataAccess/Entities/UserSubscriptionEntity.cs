using System;
using SQLite;

namespace RewriteMe.DataAccess.Entities
{
    [Table("UserSubscription")]
    public class UserSubscriptionEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        [MaxLength(100)]
        public string Time { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
