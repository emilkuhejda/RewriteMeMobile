using System;
using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUserSubscriptionService
    {
        Task SubtractTimeAsync(TimeSpan time);

        Task UpdateRemainingTimeAsync(TimeSpan remainingTime);

        Task<TimeSpan> GetRemainingTimeAsync();
    }
}
