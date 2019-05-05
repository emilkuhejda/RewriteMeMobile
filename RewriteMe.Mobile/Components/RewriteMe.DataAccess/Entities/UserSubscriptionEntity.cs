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

        public TimeSpan Time { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
