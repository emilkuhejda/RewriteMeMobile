using System;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class TimeSpanWrapper
    {
        public TimeSpan Time => TimeSpan.FromTicks(Ticks);
    }
}
