using System;
using RewriteMe.Common.Utils;

namespace RewriteMe.Domain.WebApi.Models
{
    public partial class FileItem
    {
        public TimeSpan TotalTime => TimeSpanHelper.Parse(TotalTimeString);
    }
}
