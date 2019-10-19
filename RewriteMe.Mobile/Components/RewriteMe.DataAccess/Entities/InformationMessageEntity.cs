using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace RewriteMe.DataAccess.Entities
{
    [Table("InformationMessage")]
    public class InformationMessageEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public bool IsUserSpecific { get; set; }

        public bool WasOpened { get; set; }

        public DateTime DateUpdatedUtc { get; set; }

        public DateTime DatePublishedUtc { get; set; }

        public bool IsPendingSynchronization { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public LanguageVersionEntity[] LanguageVersions { get; set; }
    }
}
