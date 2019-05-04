using System;
using RewriteMe.Common.Utils;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class TranscribeItem
    {
        public TimeSpan StartTime => TimeSpanHelper.Parse(StartTimeString);

        public TimeSpan EndTime => TimeSpanHelper.Parse(EndTimeString);

        public TimeSpan TotalTime => TimeSpanHelper.Parse(TotalTimeString);

        public bool IsPendingSynchronization { get; set; }
    }
}
