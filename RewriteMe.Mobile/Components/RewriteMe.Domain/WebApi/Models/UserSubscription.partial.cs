using System;
using RewriteMe.Common.Utils;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class UserSubscription
    {
        public bool IsEmpty => Id == Guid.Empty;

        public TimeSpan Time => TimeSpanHelper.Parse(TimeString);
    }
}
