using System;
using Newtonsoft.Json;

namespace RewriteMe.Domain.WebApi
{
    public partial class RecognitionWordInfo
    {
        [JsonIgnore]
        public TimeSpan StartTime => TimeSpan.FromTicks(StartTimeTicks);

        [JsonIgnore]
        public TimeSpan EndTime => TimeSpan.FromTicks(EndTimeTicks);
    }
}
