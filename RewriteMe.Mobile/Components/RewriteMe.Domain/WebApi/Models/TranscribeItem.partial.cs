using System;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class TranscribeItem
    {
        public TimeSpan StartTime => new TimeSpan(StartTimeTicks);

        public TimeSpan EndTime => new TimeSpan(EndTimeTicks);

        public TimeSpan TotalTime => new TimeSpan(TotalTimeTicks);

        public bool IsPendingSynchronization { get; set; }
    }
}
