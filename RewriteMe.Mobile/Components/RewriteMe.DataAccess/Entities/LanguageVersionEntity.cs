using System;
using RewriteMe.Domain.Enums;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace RewriteMe.DataAccess.Entities
{
    [Table("LanguageVersion")]
    public class LanguageVersionEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [ForeignKey(typeof(InformationMessageEntity))]
        public Guid InformationMessageId { get; set; }

        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(150)]
        public string Message { get; set; }

        public string Description { get; set; }

        public Language Language { get; set; }

        public bool SentOnOsx { get; set; }

        public bool SentOnAndroid { get; set; }
    }
}
