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
                TimeString = entity.Time.ToString(),
                DateCreated = entity.DateCreated
            };
        }

        public static UserSubscriptionEntity ToUserSubscriptionEntity(this UserSubscription userSubscription)
        {
            return new UserSubscriptionEntity
            {
                Id = userSubscription.Id,
                UserId = userSubscription.UserId,
                Time = userSubscription.Time,
                DateCreated = userSubscription.DateCreated
            };
        }
    }
}
