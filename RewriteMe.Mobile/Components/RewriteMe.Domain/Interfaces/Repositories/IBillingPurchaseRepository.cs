using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IBillingPurchaseRepository
    {
        Task AddAsync(BillingPurchase billingPurchase);

        Task DeleteAsync(Guid billingPurchaseId);

        Task<IEnumerable<BillingPurchase>> GetAllAsync(Guid userId);
    }
}
