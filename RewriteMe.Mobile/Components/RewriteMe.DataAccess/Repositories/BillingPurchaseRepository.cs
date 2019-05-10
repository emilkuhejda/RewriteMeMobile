using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.DataAccess.Repositories
{
    public class BillingPurchaseRepository : IBillingPurchaseRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public BillingPurchaseRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task AddAsync(BillingPurchase billingPurchase)
        {
            await _contextProvider.Context.InsertAsync(billingPurchase.ToBillingPurchaseEntity()).ConfigureAwait(false);
        }

        public async Task DeleteAsync(Guid billingPurchaseId)
        {
            await _contextProvider.Context.DeleteAsync(billingPurchaseId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<BillingPurchase>> GetAllAsync(Guid userId)
        {
            var entities = await _contextProvider.Context
                .BillingPurchases
                .Where(x => x.UserId == userId)
                .ToListAsync()
                .ConfigureAwait(false);

            return entities.Select(x => x.ToBillingPurchase());
        }
    }
}
