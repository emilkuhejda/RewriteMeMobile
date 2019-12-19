using System;

namespace RewriteMe.Domain.WebApi
{
    public partial class TranscribeItem
    {
        public TimeSpan StartTime => TimeSpan.FromTicks(StartTimeTicks);

        public TimeSpan EndTime => TimeSpan.FromTicks(EndTimeTicks);

        public TimeSpan TotalTime => TimeSpan.FromTicks(TotalTimeTicks);

        public bool IsPendingSynchronization { get; set; }
    }
}
