using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using FFImageLoading.Forms.Platform;
using Microsoft.Identity.Client;
using Plugin.InAppBilling;
using RewriteMe.Business.Configuration;
using RewriteMe.Mobile.Droid.Configuration;
using RewriteMe.Mobile.Droid.Utils;
using Xamarin.Forms;

namespace RewriteMe.Mobile.Droid
{
    [Activity(
        Label = "@string/ApplicationName",
        Icon = "@mipmap/ic_launcher",
        MainLauncher = false,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Forms.SetFlags("CollectionView_Experimental");
            Forms.Init(this, savedInstanceState);

            CachedImageRenderer.Init(null);
            UserDialogs.Init(this);

            InitializationParameters.Current.ImportedFilePath = Intent.GetStringExtra(ExtraConstants.FilePath);

            var bootstrapper = new AndroidBootstrapper();
            var application = new App(bootstrapper);
            LoadApplication(application);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.RecordAudio }, 1);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }
    }
}