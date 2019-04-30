using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IUserSessionRepository
    {
        Task<UserSession> GetUserSessionAsync();

        Task UpdateUserSessionAsync(UserSession userSession);

        Task ClearAsync();

        Task<bool> UserSessionExistsAsync();
    }
}
