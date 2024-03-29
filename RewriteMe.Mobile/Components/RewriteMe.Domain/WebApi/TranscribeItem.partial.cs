﻿using System;
using System.Linq;

namespace RewriteMe.Domain.WebApi
{
    public partial class TranscribeItem
    {
        public string Transcript => Alternatives.Any() ? string.Join(string.Empty, Alternatives.Select(x => x.Transcript)) : null;

        public TimeSpan StartTime => TimeSpan.FromTicks(StartTimeTicks);

        public TimeSpan EndTime => TimeSpan.FromTicks(EndTimeTicks);

        public TimeSpan TotalTime => TimeSpan.FromTicks(TotalTimeTicks);

        public string TimeRange
        {
            get
            {
                if (StartTime.TotalHours >= 1)
                {
                    return $"{StartTime:hh\\:mm\\:ss} - {EndTime:hh\\:mm\\:ss}";
                }

                return $"{StartTime:mm\\:ss} - {EndTime:mm\\:ss}";
            }
        }

        public bool IsPendingSynchronization { get; set; }
    }
}
