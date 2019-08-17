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

        Task<B2CAccessToken> SignUpOrInAsync();

        Task<B2CAccessToken> EditProfileAsync();

        Task<B2CAccessToken> ResetPasswordAsync();

        Task RegisterUserAsync(B2CAccessToken accessToken);

        Task SignOutAsync();
    }
}
