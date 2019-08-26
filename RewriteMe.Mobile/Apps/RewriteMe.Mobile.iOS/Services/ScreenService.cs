using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Mobile.Utils;
using UIKit;

namespace RewriteMe.Mobile.iOS.Services
{
    public class ScreenService : IScreenService
    {
        public void DisableIdle()
        {
            ThreadHelper.InvokeOnUiThread(() => UIApplication.SharedApplication.IdleTimerDisabled = true);
        }

        public void EnableIdle()
        {
            ThreadHelper.InvokeOnUiThread(() => UIApplication.SharedApplication.IdleTimerDisabled = false);
        }
    }
}