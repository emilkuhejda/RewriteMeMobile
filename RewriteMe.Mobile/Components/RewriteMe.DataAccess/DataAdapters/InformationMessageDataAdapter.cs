using System.Linq;
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
                IsUserSpecific = entity.IsUserSpecific,
                WasOpened = entity.WasOpened,
                DateUpdated = entity.DateUpdated,
                DatePublished = entity.DatePublished,
                IsPendingSynchronization = entity.IsPendingSynchronization,
                LanguageVersions = entity.LanguageVersions?.Select(x => x.ToLanguageVersion()).ToList()
            };
        }

        public static InformationMessageEntity ToInformationMessageEntity(this InformationMessage informationMessage)
        {
            return new InformationMessageEntity
            {
                Id = informationMessage.Id,
                IsUserSpecific = informationMessage.IsUserSpecific,
                WasOpened = informationMessage.WasOpened,
                DateUpdated = informationMessage.DateUpdated,
                DatePublished = informationMessage.DatePublished,
                IsPendingSynchronization = informationMessage.IsPendingSynchronization,
                LanguageVersions = informationMessage.LanguageVersions?.Select(x => x.ToLanguageVersionEntity()).ToArray()
            };
        }
    }
}
