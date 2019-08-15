using System;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IAppCenterMetricsService
    {
        void TrackException(Exception exception);
    }
}
