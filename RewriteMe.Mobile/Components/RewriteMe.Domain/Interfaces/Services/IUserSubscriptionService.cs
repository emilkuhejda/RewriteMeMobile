using System;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUserSubscriptionService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task RecognizedTimeSynchronizationAsync();

        Task AddAsync(UserSubscription userSubscription);

        Task<TimeSpan> GetRemainingTimeAsync();
    }
}
