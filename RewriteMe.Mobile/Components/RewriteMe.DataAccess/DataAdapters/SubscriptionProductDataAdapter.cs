using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class SubscriptionProductDataAdapter
    {
        public static SubscriptionProduct ToSubscriptionProduct(this SubscriptionProductEntity entity)
        {
            return new SubscriptionProduct
            {
                Id = entity.Id,
                TimeString = entity.ToString()
            };
        }

        public static SubscriptionProductEntity ToSubscriptionProductEntity(this SubscriptionProduct subscriptionProduct)
        {
            return new SubscriptionProductEntity
            {
                Id = subscriptionProduct.Id,
                Time = subscriptionProduct.Time
            };
        }
    }
}
