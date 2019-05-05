using System;

namespace RewriteMe.Domain.Interfaces.Required
{
    public interface IMediaService
    {
        TimeSpan GetDuration(string fileName);
    }
}
