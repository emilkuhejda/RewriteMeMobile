using System;
using RewriteMe.Common.Utils;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class UserSubscription
    {
        public bool IsEmpty => !Id.HasValue || Id.Value == Guid.Empty;

        public TimeSpan Time => TimeSpanHelper.Parse(TimeString);
    }
}
