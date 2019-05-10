using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface ISubscriptionProductRepository
    {
        Task<IEnumerable<SubscriptionProduct>> GetAsync();

        Task AddAsync(SubscriptionProduct subscriptionProduct);

        Task ReplaceAllAsync(IEnumerable<SubscriptionProduct> subscriptionProducts);

        Task ClearAsync();
    }
}
