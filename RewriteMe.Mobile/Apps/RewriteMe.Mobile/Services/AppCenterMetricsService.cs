using System;
using Microsoft.AppCenter.Crashes;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Mobile.Services
{
    public class AppCenterMetricsService : IAppCenterMetricsService
    {
        public void TrackException(Exception exception)
        {
            Crashes.TrackError(exception);
        }
    }
}
