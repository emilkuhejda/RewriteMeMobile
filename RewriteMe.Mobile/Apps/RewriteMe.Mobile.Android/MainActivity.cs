using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Platform;
using RewriteMe.Mobile.Droid.Configuration;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Droid
{
    [Activity(
        Label = "@string/ApplicationName",
        Icon = "@mipmap/icon",
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Forms.Init(this, savedInstanceState);

            CachedImageRenderer.Init(null);
            UserDialogs.Init(this);

            var bootstrapper = new AndroidBootstrapper();
            LoadApplication(new App(bootstrapper));
        }
    }
}