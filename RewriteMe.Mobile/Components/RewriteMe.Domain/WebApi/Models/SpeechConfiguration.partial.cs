using System;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class SpeechConfiguration
    {
        public TimeSpan SubscriptionRemainingTime => new TimeSpan(SubscriptionRemainingTimeTicks);
    }
}
