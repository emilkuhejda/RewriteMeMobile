﻿using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace RewriteMe.DataAccess.Entities
{
    [Table("InformationMessage")]
    public class InformationMessageEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public LanguageVersionEntity[] LanguageVersions { get; set; }
    }
}
