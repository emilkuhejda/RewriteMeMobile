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
    public class SubscriptionProductRepository : ISubscriptionProductRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public SubscriptionProductRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<IEnumerable<SubscriptionProduct>> GetAsync()
        {
            var entities = await _contextProvider.Context.SubscriptionProducts.ToListAsync().ConfigureAwait(false);

            return entities.Select(x => x.ToSubscriptionProduct());
        }

        public async Task AddAsync(SubscriptionProduct subscriptionProduct)
        {
            await _contextProvider.Context.InsertAsync(subscriptionProduct.ToSubscriptionProductEntity()).ConfigureAwait(false);
        }

        public async Task ReplaceAllAsync(IEnumerable<SubscriptionProduct> subscriptionProducts)
        {
            await _contextProvider.Context.RunInTransactionAsync(database =>
            {
                database.DeleteAll<SubscriptionProductEntity>();
                database.InsertAll(subscriptionProducts.Select(x => x.ToSubscriptionProductEntity()));
            }).ConfigureAwait(false);
        }

        public async Task ClearAsync()
        {
            await _contextProvider.Context.DeleteAllAsync<SubscriptionProductEntity>().ConfigureAwait(false);
        }
    }
}
