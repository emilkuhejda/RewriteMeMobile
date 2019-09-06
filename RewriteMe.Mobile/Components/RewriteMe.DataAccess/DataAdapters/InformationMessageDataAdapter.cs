﻿using System.Linq;
using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class InformationMessageDataAdapter
    {
        public static InformationMessage ToInformationMessage(this InformationMessageEntity entity)
        {
            return new InformationMessage
            {
                Id = entity.Id,
                DateCreated = entity.DateCreated,
                LanguageVersions = entity.LanguageVersions?.Select(x => x.ToLanguageVersion()).ToList()
            };
        }

        public static InformationMessageEntity ToInformationMessageEntity(this InformationMessage informationMessage)
        {
            return new InformationMessageEntity
            {
                Id = informationMessage.Id.GetValueOrDefault(),
                DateCreated = informationMessage.DateCreated.GetValueOrDefault(),
                LanguageVersions = informationMessage.LanguageVersions?.Select(x => x.ToLanguageVersionEntity()).ToArray()
            };
        }
    }
}
