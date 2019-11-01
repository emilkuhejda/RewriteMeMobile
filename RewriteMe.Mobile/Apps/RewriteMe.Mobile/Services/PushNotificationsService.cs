﻿using System.Threading.Tasks;
using Microsoft.AppCenter.Push;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Mobile.Services
{
    public class PushNotificationsService : IPushNotificationsService
    {
        public async Task<bool> IsEnabledAsync()
        {
            return await Push.IsEnabledAsync().ConfigureAwait(false);
        }

        public async Task SetEnabledAsync(bool enabled)
        {
            await Push.SetEnabledAsync(enabled).ConfigureAwait(false);
        }
    }
}
