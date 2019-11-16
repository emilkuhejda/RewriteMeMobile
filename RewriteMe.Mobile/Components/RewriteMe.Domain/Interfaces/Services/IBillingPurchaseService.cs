using System.Threading.Tasks;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IBillingPurchaseService
    {
        Task<RemainingTime> SendBillingPurchaseAsync(BillingPurchase billingPurchase);
    }
}
