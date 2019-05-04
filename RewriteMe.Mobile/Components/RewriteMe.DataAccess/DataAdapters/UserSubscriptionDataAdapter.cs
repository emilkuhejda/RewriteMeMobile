using RewriteMe.DataAccess.Entities;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.DataAccess.DataAdapters
{
    public static class UserSubscriptionDataAdapter
    {
        public static UserSubscription ToUserSubscription(this UserSubscriptionEntity entity)
        {
            return new UserSubscription
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Time = entity.Time.ToString(),
                DateCreated = entity.DateCreated
            };
        }

        public static UserSubscriptionEntity ToUserSubscriptionEntity(this UserSubscription userSubscription)
        {
            return new UserSubscriptionEntity
            {
                Id = userSubscription.Id.GetValueOrDefault(),
                UserId = userSubscription.UserId.GetValueOrDefault(),
                Time = userSubscription.SubscriptionTime,
                DateCreated = userSubscription.DateCreated.GetValueOrDefault()
            };
        }
    }
}
