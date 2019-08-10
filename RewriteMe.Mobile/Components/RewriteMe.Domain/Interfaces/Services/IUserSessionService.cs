using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUserSessionService
    {
        Task<Guid> GetUserIdAsync();

        Task<string> GetUserNameAsync();

        Task<UserSession> GetUserSessionAsync();

        string GetAccessToken();

        void SetAccessToken(string accessToken);

        Task<bool> IsSignedInAsync();

        Task<bool> SignUpOrInAsync();

        Task<bool> EditProfileAsync();

        Task<bool> ResetPasswordAsync();

        Task SignOutAsync();
    }
}
