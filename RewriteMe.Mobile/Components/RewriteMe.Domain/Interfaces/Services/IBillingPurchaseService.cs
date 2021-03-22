using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.InAppBilling;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IBillingPurchaseService
    {
        Task<IEnumerable<InAppBillingPurchase>> GetAllPaymentPendingAsync();

        Task AddAsync(InAppBillingPurchase billingPurchase);

        Task<TimeSpanWrapper> SendBillingPurchaseAsync(CreateUserSubscriptionInputModel createUserSubscriptionInputModel);

        Task HandlePendingPurchases();
    }
}
