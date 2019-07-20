using System;
using RewriteMe.Common.Utils;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class SpeechConfiguration
    {
        public TimeSpan SubscriptionRemainingTime => TimeSpanHelper.Parse(SubscriptionRemainingTimeString);
    }
}
