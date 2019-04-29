using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.Configuration;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class UserSessionDataAdapter
    {
        public static UserSession ToUserSession(this UserSessionEntity entity)
        {
            return new UserSession
            {
                ObjectId = entity.ObjectId,
                Email = entity.Email,
                GivenName = entity.GivenName,
                FamilyName = entity.FamilyName
            };
        }

        public static UserSessionEntity ToUserSessionEntity(this UserSession userSession)
        {
            return new UserSessionEntity
            {
                ObjectId = userSession.ObjectId,
                Email = userSession.Email,
                GivenName = userSession.GivenName,
                FamilyName = userSession.FamilyName
            };
        }
    }
}
