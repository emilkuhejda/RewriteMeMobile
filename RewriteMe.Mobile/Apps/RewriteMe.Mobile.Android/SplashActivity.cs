using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using RewriteMe.Business.Configuration;
using RewriteMe.Mobile.Droid.Extensions;

namespace RewriteMe.Mobile.Droid
{
    [Activity(
        Label = "@string/ApplicationName",
        Icon = "@mipmap/ic_launcher",
        Theme = "@style/SplashTheme",
        MainLauncher = true,
        NoHistory = true,
        LaunchMode = LaunchMode.SingleTask)]
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
            if (Intent.Extras != null)
                intent.PutExtras(Intent.Extras);

            if (Intent.Action == Intent.ActionSend)
            {
                byte[] bytes;
                var path = Intent.ClipData.GetItemAt(0);
                var stream = ContentResolver.OpenInputStream(path.Uri);
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }

                var filePath = path.Uri.GetPath(ContentResolver);
                var fileName = Path.GetFileName(Uri.UnescapeDataString(filePath));
                InitializationParameters.Current.ImportedFileName = fileName;
                InitializationParameters.Current.ImportedFileSource = bytes;
            }

            StartActivity(intent);
            Finish();
        }
    }
}