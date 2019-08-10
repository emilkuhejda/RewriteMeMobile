using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUserSessionService
    {
        AccessToken AccessToken { get; }

        Task<Guid> GetUserIdAsync();

        Task<string> GetUserNameAsync();

        Task<UserSession> GetUserSessionAsync();

        string GetToken();

        void SetToken(string accessToken);

        Task<bool> IsSignedInAsync();

        Task<bool> SignUpOrInAsync();

        Task<bool> EditProfileAsync();

        Task<bool> ResetPasswordAsync();

        Task SignOutAsync();
    }
}
