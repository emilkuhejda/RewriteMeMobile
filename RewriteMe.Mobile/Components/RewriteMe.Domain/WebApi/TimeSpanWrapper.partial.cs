using System;

namespace RewriteMe.Domain.WebApi
{
    public partial class TimeSpanWrapper
    {
        public TimeSpan Time => TimeSpan.FromTicks(Ticks);
    }
}
