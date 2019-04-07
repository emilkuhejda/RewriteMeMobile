using System;
using System.Diagnostics;
using System.Threading;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Utils
{
    public static class ThreadHelper
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(1);

        public static void InvokeOnUiThread(Action action)
        {
            if (SynchronizationContext.Current != null)
            {
                // Already in UI thread.
                action();
            }
            else
            {
                // Not on UI thread
                var resetEvent = new ManualResetEvent(false);
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        action();
                    }
                    finally
                    {
                        resetEvent.Set();
                    }
                });

                var timeout = Debugger.IsAttached ? TimeSpan.FromDays(1) : Timeout;
                if (!resetEvent.WaitOne(timeout))
                {
                    throw new TimeoutException("Possible deadlock detected.");
                }
            }
        }

        public static T InvokeOnUiThread<T>(Func<T> action)
        {
            if (SynchronizationContext.Current != null)
            {
                // Already in UI thread.
                return action();
            }

            // Not on UI thread
            var resetEvent = new ManualResetEvent(false);
            var result = default(T);
            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    result = action();
                }
                finally
                {
                    resetEvent.Set();
                }
            });
            if (!resetEvent.WaitOne(Timeout))
            {
                throw new TimeoutException("Possible deadlock detected.");
            }
            return result;
        }
    }
}
