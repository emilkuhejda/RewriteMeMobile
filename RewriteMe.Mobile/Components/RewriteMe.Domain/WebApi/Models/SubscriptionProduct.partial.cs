using System;
using RewriteMe.Common.Utils;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class SubscriptionProduct
    {
        public TimeSpan Time => TimeSpanHelper.Parse(TimeString);
    }
}
