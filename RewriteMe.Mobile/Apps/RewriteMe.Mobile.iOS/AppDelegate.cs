using System;
using System.IO;
using FFImageLoading.Forms.Platform;
using Foundation;
using RewriteMe.Mobile.iOS.Configuration;
using RewriteMe.Mobile.iOS.Utils;
using UIKit;
using Xamarin.Forms;

namespace RewriteMe.Mobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private App _application;

        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Forms.Init();
            CachedImageRenderer.Init();

            var bootstrapper = new OSXBootstrapper();
            _application = new App(bootstrapper);
            LoadApplication(_application);

            InitializeStatusBarColor();

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            byte[] source = null;
            if (File.Exists(url.Path))
            {
                source = File.ReadAllBytes(url.Path);
            }

            var fileName = Path.GetFileName(Uri.UnescapeDataString(url.Path));
            _application.ImportFile(fileName, source);

            return true;
        }

        private void InitializeStatusBarColor()
        {
            using (var nsString = new NSString("statusBar"))
            {
                var statusBar = UIApplication.SharedApplication.ValueForKey(nsString) as UIView;
                if (statusBar != null && statusBar.RespondsToSelector(new ObjCRuntime.Selector("setBackgroundColor:")))
                {
                    statusBar.BackgroundColor = Colors.Primary;
                }
            }
        }
    }
}
