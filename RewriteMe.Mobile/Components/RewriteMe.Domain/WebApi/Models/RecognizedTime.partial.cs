using System;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class RecognizedTime
    {
        public TimeSpan TotalTime => new TimeSpan(TotalTimeTicks);
    }
}
