using System.Linq;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Interfaces.Repositories;

namespace RewriteMe.DataAccess.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public UserSessionRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<UserSession> GetUserSessionAsync()
        {
            var userSessions = await _contextProvider.Context.UserSessions.ToListAsync().ConfigureAwait(false);
            var userSession = userSessions.SingleOrDefault();
            if (userSession == null)
                return new UserSession();

            return userSession.ToUserSession();
        }

        public async Task UpdateUserSessionAsync(UserSession userSession)
        {
            await _contextProvider.Context.RunInTransactionAsync(database =>
            {
                database.DeleteAll<UserSessionEntity>();
                database.Insert(userSession.ToUserSessionEntity());
            }).ConfigureAwait(false);
        }

        public async Task ClearUserSessionAsync()
        {
            await _contextProvider.Context.DeleteAllAsync<UserSessionEntity>().ConfigureAwait(false);
        }

        public async Task<bool> UserSessionExistsAsync()
        {
            var userSessions = await _contextProvider.Context.UserSessions.ToListAsync().ConfigureAwait(false);
            return userSessions.Any();
        }
    }
}
