using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.InAppBilling;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IBillingPurchaseRepository
    {
        Task<IEnumerable<InAppBillingPurchase>> GetAllAsync();

        Task<IEnumerable<InAppBillingPurchase>> GetAllPaymentPendingAsync();

        Task AddAsync(InAppBillingPurchase billingPurchase);

        Task UpdateAsync(InAppBillingPurchase billingPurchase);

        Task DeleteAsync(Guid billingPurchaseId);
    }
}
