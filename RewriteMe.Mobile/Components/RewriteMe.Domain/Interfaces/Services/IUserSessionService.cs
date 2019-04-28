using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUserSessionService
    {
        Task<string> GetUserIdAsync();

        Task<string> GetUserNameAsync();

        Task<string> GetAccessTokenSilentAsync();

        Task<bool> IsSignedInAsync();

        Task<bool> SignUpOrInAsync();

        Task<bool> EditProfileAsync();

        Task<bool> ResetPasswordAsync();

        Task SignOutAsync();
    }
}
