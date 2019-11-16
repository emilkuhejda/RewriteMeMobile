using System;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class RemainingTime
    {
        public TimeSpan Time => TimeSpan.FromTicks(TimeTicks);
    }
}
