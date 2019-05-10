using System;
using SQLite;

namespace RewriteMe.DataAccess.Entities
{
    [Table("SubscriptionProduct")]
    public class SubscriptionProductEntity
    {
        [PrimaryKey]
        public string Id { get; set; }

        public TimeSpan Time { get; set; }
    }
}
