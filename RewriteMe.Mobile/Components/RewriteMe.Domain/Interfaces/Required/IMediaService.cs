using System;

namespace RewriteMe.Domain.Interfaces.Required
{
    public interface IMediaService
    {
        TimeSpan GetTotalTime(string fileName);
    }
}
