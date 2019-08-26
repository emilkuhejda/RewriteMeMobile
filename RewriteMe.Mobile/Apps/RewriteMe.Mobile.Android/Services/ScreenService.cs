using Android.Views;
using Plugin.CurrentActivity;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.Droid.Services
{
    public class ScreenService : IScreenService
    {
        public void DisableIdle()
        {
            CrossCurrentActivity.Current.Activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn);
        }

        public void EnableIdle()
        {
            CrossCurrentActivity.Current.Activity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn);
        }
    }
}