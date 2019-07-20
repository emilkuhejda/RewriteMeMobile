using System;
using RewriteMe.Common.Utils;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class RecognizedTime
    {
        public TimeSpan TotalTime => TimeSpanHelper.Parse(TotalTimeString);
    }
}
