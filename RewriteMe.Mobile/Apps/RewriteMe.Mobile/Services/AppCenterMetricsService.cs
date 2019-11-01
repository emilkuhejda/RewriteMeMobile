using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
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

        public void TrackEvent(string name, Dictionary<string, string> properties)
        {
            Analytics.TrackEvent(name, properties);
        }
    }
}
