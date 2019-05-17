using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Extensions
{
    public static class UserSessionExtensions
    {
        public static RegisterUserModel ToRegisterUserModel(this UserSession userSession)
        {
            return new RegisterUserModel
            {
                Id = userSession.ObjectId,
                Email = userSession.Email,
                GivenName = userSession.GivenName,
                FamilyName = userSession.FamilyName
            };
        }
    }
}
