using System;
using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUserSubscriptionService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate);

        Task<TimeSpan> GetRemainingTimeAsync();
    }
}
