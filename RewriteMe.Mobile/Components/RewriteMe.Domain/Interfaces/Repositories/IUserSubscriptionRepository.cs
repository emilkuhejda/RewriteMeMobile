using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IUserSubscriptionRepository
    {
        Task AddAsync(UserSubscription userSubscription);

        Task InsertOrReplaceAllAsync(IEnumerable<UserSubscription> userSubscriptions);

        Task<TimeSpan> GetTotalTimeAsync();

        Task ClearAsync();
    }
}
