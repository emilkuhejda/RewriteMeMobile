using Android.Views;
using Plugin.CurrentActivity;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Mobile.Utils;

namespace RewriteMe.Mobile.Droid.Services
{
    public class ScreenService : IScreenService
    {
        public void DisableIdle()
        {
            ThreadHelper.InvokeOnUiThread(() => CrossCurrentActivity.Current.Activity.Window.AddFlags(WindowManagerFlags.KeepScreenOn));
        }

        public void EnableIdle()
        {
            ThreadHelper.InvokeOnUiThread(() => CrossCurrentActivity.Current.Activity.Window.ClearFlags(WindowManagerFlags.KeepScreenOn));
        }
    }
}