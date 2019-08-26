using RewriteMe.Domain.Interfaces.Required;
using UIKit;

namespace RewriteMe.Mobile.iOS.Services
{
    public class ScreenService : IScreenService
    {
        public void DisableIdle()
        {
            UIApplication.SharedApplication.IdleTimerDisabled = true;
        }

        public void EnableIdle()
        {
            UIApplication.SharedApplication.IdleTimerDisabled = false;
        }
    }
}