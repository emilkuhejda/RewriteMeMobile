using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using RewriteMe.Mobile.Droid.Utils;

namespace RewriteMe.Mobile.Droid
{
    [Activity(
        Label = "@string/ApplicationName",
        Icon = "@mipmap/ic_launcher",
        Theme = "@style/SplashTheme",
        MainLauncher = true,
        NoHistory = true)]
    [IntentFilter(
        new[] { Intent.ActionSend },
        Categories = new[] { Intent.CategoryDefault },
        DataMimeType = @"audio/*")]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var intent = new Intent(this, typeof(MainActivity));

            if (Intent.Action == Intent.ActionSend)
            {
                var path = Intent.ClipData.GetItemAt(0);
                intent.PutExtra(ExtraConstants.FileUri, path.Uri);
            }

            StartActivity(intent);
            Finish();
        }
    }
}