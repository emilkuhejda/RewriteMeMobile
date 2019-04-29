﻿using SQLite;

namespace RewriteMe.DataAccess.Entities
{
    [Table("UserSession")]
    public class UserSessionEntity
    {
        [PrimaryKey]
        public string ObjectId { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string GivenName { get; set; }

        [MaxLength(100)]
        public string FamilyName { get; set; }
    }
}
