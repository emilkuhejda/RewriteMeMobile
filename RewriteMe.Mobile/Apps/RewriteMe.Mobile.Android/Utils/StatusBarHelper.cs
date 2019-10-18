using Android.OS;
using Android.Views;
using Plugin.CurrentActivity;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Droid.Utils
{
    public static class StatusBarHelper
    {
        public static void SetLightStatusBar()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var currentWindow = GetCurrentWindow();
                    currentWindow.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                });
            }
        }

        private static Window GetCurrentWindow()
        {
            var window = CrossCurrentActivity.Current.Activity.Window;

            // Clear FLAG_TRANSLUCENT_STATUS flag:
            window.ClearFlags(WindowManagerFlags.TranslucentStatus);

            // Add FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS flag to the window
            window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

            return window;
        }
    }
}