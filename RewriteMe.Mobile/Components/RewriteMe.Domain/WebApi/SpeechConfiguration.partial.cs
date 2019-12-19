using System;

namespace RewriteMe.Domain.WebApi
{
    public partial class SpeechConfiguration
    {
        public TimeSpan SubscriptionRemainingTime => TimeSpan.FromTicks(SubscriptionRemainingTimeTicks);
    }
}
