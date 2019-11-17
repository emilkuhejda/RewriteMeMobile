using System;
using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IUserSubscriptionSynchronizationService
    {
        Task SynchronizationAsync(DateTime applicationUpdateDate);
    }
}
