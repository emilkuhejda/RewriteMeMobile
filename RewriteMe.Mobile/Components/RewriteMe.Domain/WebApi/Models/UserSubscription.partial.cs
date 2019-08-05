using System;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class UserSubscription
    {
        public bool IsEmpty => Id == Guid.Empty;

        public TimeSpan Time => TimeSpan.FromTicks(TimeTicks);
    }
}
