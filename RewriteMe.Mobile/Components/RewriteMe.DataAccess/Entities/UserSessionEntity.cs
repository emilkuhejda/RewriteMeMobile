using SQLite;

namespace RewriteMe.DataAccess.Entities
{
    public class UserSessionEntity
    {
        [PrimaryKey]
        public string ObjectId { get; set; }

        [MaxLength(100)]
        public string GivenName { get; set; }

        [MaxLength(100)]
        public string FamilyName { get; set; }
    }
}
