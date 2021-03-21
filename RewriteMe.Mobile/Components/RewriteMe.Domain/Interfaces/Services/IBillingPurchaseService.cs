using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.InAppBilling;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IBillingPurchaseService
    {
        Task<IEnumerable<InAppBillingPurchase>> GetAllAsync();

        Task<IEnumerable<InAppBillingPurchase>> GetAllPaymentPendingAsync();

        Task AddAsync(InAppBillingPurchase billingPurchase);

        Task UpdateAsync(InAppBillingPurchase billingPurchase);

        Task DeleteAsync(string billingPurchaseId);

        Task<TimeSpanWrapper> SendBillingPurchaseAsync(CreateUserSubscriptionInputModel createUserSubscriptionInputModel);
    }
}
