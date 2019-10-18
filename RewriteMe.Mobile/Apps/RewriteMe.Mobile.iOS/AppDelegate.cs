using System;
using System.IO;
using FFImageLoading.Forms.Platform;
using Foundation;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Messages;
using RewriteMe.Mobile.iOS.BackgroundServices;
using RewriteMe.Mobile.iOS.Configuration;
using Syncfusion.SfBusyIndicator.XForms.iOS;
using Syncfusion.XForms.iOS.BadgeView;
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
        private TranscribeItemBackgroundService _transcribeItemBackgroundService;
        private SynchronizerBackgroundService _synchronizerBackgroundService;

        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Forms.Init();
            CachedImageRenderer.Init();
            SfBadgeViewRenderer.Init();

            InitializeBusyIndicatorRenderer();

            var bootstrapper = new OsxBootstrapper();
            _application = new App(bootstrapper);
            LoadApplication(_application);

            WireUpBackgroundServices();

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

        private void WireUpBackgroundServices()
        {
            MessagingCenter.Subscribe<StartBackgroundServiceMessage>(this, nameof(BackgroundServiceType.TranscribeItem),
                async message =>
                {
                    _transcribeItemBackgroundService = new TranscribeItemBackgroundService();
                    await _transcribeItemBackgroundService.RunAsync().ConfigureAwait(false);
                });

            MessagingCenter.Subscribe<StartBackgroundServiceMessage>(this, nameof(BackgroundServiceType.Synchronizer),
                async message =>
                {
                    _synchronizerBackgroundService = new SynchronizerBackgroundService();
                    await _synchronizerBackgroundService.RunAsync().ConfigureAwait(false);
                });

            MessagingCenter.Subscribe<StopBackgroundServiceMessage>(this, nameof(BackgroundServiceType.TranscribeItem), message => { _transcribeItemBackgroundService.Stop(); });
            MessagingCenter.Subscribe<StopBackgroundServiceMessage>(this, nameof(BackgroundServiceType.Synchronizer), message => { _synchronizerBackgroundService.Stop(); });
        }

        private void InitializeBusyIndicatorRenderer()
        {
            using (var busyIndicatorRenderer = new SfBusyIndicatorRenderer()) { }
        }
    }
}
