using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.DataAccess.Repositories
{
    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public UserSubscriptionRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task InsertOrReplaceAllAsync(IEnumerable<UserSubscription> userSubscriptions)
        {
            var userSubscriptionEntities = userSubscriptions.Select(x => x.ToUserSubscriptionEntity()).ToList();
            if (!userSubscriptionEntities.Any())
                return;

            var existingEntities = await _contextProvider.Context.UserSubscriptions.ToListAsync().ConfigureAwait(false);
            var mergedFileItems = existingEntities.Where(x => userSubscriptionEntities.All(e => e.Id != x.Id)).ToList();
            mergedFileItems.AddRange(userSubscriptionEntities);

            await _contextProvider.Context.RunInTransactionAsync(database =>
            {
                database.DeleteAll<UserSubscriptionEntity>();
                database.InsertAll(mergedFileItems);
            }).ConfigureAwait(false);
        }
    }
}
