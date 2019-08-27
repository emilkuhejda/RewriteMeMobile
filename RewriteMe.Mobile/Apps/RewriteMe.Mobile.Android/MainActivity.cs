using System.IO;
using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using FFImageLoading.Forms.Platform;
using Microsoft.AppCenter.Push;
using Microsoft.Identity.Client;
using Plugin.InAppBilling;
using RewriteMe.Business.Configuration;
using RewriteMe.Mobile.Droid.Configuration;
using RewriteMe.Mobile.Droid.Extensions;
using RewriteMe.Mobile.Droid.Utils;
using Xamarin.Forms;
using SystemUri = System.Uri;

namespace RewriteMe.Mobile.Droid
{
    [Activity(
        Label = "@string/ApplicationName",
        Icon = "@mipmap/ic_launcher",
        MainLauncher = false,
        LaunchMode = LaunchMode.SingleInstance,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Forms.Init(this, savedInstanceState);

            CachedImageRenderer.Init(null);
            UserDialogs.Init(this);

            InitializeSharedFile();

            var bootstrapper = new AndroidBootstrapper();
            var application = new App(bootstrapper);
            LoadApplication(application);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.RecordAudio }, 1);
            }
        }

        private void InitializeSharedFile()
        {
            if (!(Intent.GetParcelableExtra(ExtraConstants.FileUri) is Uri uri))
                return;

            byte[] bytes;
            var stream = ContentResolver.OpenInputStream(uri);
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            var filePath = uri.GetPath(ContentResolver);
            var fileName = Path.GetFileName(SystemUri.UnescapeDataString(filePath));
            InitializationParameters.Current.ImportedFileName = fileName;
            InitializationParameters.Current.ImportedFileSource = bytes;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            Push.CheckLaunchedFromNotification(this, intent);
        }
    }
}