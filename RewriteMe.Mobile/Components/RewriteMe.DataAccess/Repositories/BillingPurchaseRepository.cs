using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plugin.InAppBilling;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;

namespace RewriteMe.DataAccess.Repositories
{
    public class BillingPurchaseRepository : IBillingPurchaseRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public BillingPurchaseRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<IEnumerable<InAppBillingPurchase>> GetAllAsync()
        {
            var entities = await _contextProvider.Context.BillingPurchases
                .ToArrayAsync()
                .ConfigureAwait(false);

            return entities.Select(x => x.ToBillingPurchase());
        }

        public async Task<IEnumerable<InAppBillingPurchase>> GetAllPaymentPendingAsync()
        {
            var entities = await _contextProvider.Context.BillingPurchases
                .Where(x => x.State == PurchaseState.PaymentPending)
                .ToArrayAsync()
                .ConfigureAwait(false);

            return entities.Select(x => x.ToBillingPurchase());
        }

        public Task AddAsync(InAppBillingPurchase billingPurchase)
        {
            return _contextProvider.Context.InsertAsync(billingPurchase.ToBillingPurchaseEntity());
        }

        public Task UpdateAsync(InAppBillingPurchase billingPurchase)
        {
            return _contextProvider.Context.UpdateAsync(billingPurchase.ToBillingPurchaseEntity());
        }

        public Task DeleteAsync(string billingPurchaseId)
        {
            return _contextProvider.Context.DeleteAsync<BillingPurchaseEntity>(billingPurchaseId);
        }
    }
}
