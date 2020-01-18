﻿using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using FFImageLoading.Forms.Platform;
using Microsoft.AppCenter.Push;
using Microsoft.Identity.Client;
using Plugin.InAppBilling;
using RewriteMe.Domain.Enums;
using RewriteMe.Domain.Messages;
using RewriteMe.Mobile.Droid.BackgroundServices;
using RewriteMe.Mobile.Droid.Configuration;
using RewriteMe.Mobile.Droid.Utils;
using Xamarin.Forms;
using SystemUri = System.Uri;

namespace RewriteMe.Mobile.Droid
{
    [Activity(
        Label = "@string/ApplicationName",
        Icon = "@mipmap/ic_launcher",
        MainLauncher = false,
        LaunchMode = LaunchMode.SingleTask,
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

            StatusBarHelper.SetLightStatusBar();

            var bootstrapper = new AndroidBootstrapper();
            var application = new App(bootstrapper);
            LoadApplication(application);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new[] { Manifest.Permission.RecordAudio }, 1);
            }

            WireUpBackgroundServices();
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

        private void WireUpBackgroundServices()
        {
            MessagingCenter.Subscribe<StartBackgroundServiceMessage>(this, nameof(BackgroundServiceType.TranscribeItem), StartBackgroundService<TranscribeItemBackgroundService>);
            MessagingCenter.Subscribe<StartBackgroundServiceMessage>(this, nameof(BackgroundServiceType.Synchronizer), StartBackgroundService<SynchronizerBackgroundService>);
            MessagingCenter.Subscribe<StartBackgroundServiceMessage>(this, nameof(BackgroundServiceType.UploadFileItem), StartBackgroundService<FileItemUploaderBackgroundService>);
            MessagingCenter.Subscribe<StopBackgroundServiceMessage>(this, nameof(BackgroundServiceType.TranscribeItem), StopBackgroundService<TranscribeItemBackgroundService>);
            MessagingCenter.Subscribe<StopBackgroundServiceMessage>(this, nameof(BackgroundServiceType.Synchronizer), StopBackgroundService<SynchronizerBackgroundService>);
            MessagingCenter.Subscribe<StopBackgroundServiceMessage>(this, nameof(BackgroundServiceType.UploadFileItem), StopBackgroundService<FileItemUploaderBackgroundService>);
        }

        private void StartBackgroundService<T>(StartBackgroundServiceMessage message)
        {
            using (var intent = new Intent(this, typeof(T)))
            {
                StartService(intent);
            }
        }

        private void StopBackgroundService<T>(StopBackgroundServiceMessage message)
        {
            using (var intent = new Intent(this, typeof(T)))
            {
                StopService(intent);
            }
        }
    }
}