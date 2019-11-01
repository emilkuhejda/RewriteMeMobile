using System;
using System.Collections.Generic;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IAppCenterMetricsService
    {
        void TrackException(Exception exception);

        void TrackEvent(string name, Dictionary<string, string> properties);
    }
}
