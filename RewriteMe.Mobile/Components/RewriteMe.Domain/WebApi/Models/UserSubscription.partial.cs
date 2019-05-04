using System;
using RewriteMe.Common.Utils;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class UserSubscription
    {
        public TimeSpan SubscriptionTime => TimeSpanHelper.Parse(Time);
    }
}
