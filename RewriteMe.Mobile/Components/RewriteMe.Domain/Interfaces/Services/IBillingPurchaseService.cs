using System.Threading.Tasks;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IBillingPurchaseService
    {
        Task<TimeSpanWrapper> SendBillingPurchaseAsync(BillingPurchase billingPurchase);
    }
}
