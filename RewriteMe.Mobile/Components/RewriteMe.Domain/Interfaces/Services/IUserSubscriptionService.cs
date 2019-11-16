using System;
using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUserSubscriptionService
    {
        Task UpdateRemainingTimeAsync(TimeSpan remainingTime);

        Task<TimeSpan> GetRemainingTimeAsync();
    }
}
