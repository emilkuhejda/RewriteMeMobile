﻿using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Prism.Ioc;
using RewriteMe.Domain.Interfaces.Services;
using OperationCanceledException = System.OperationCanceledException;

namespace RewriteMe.Mobile.Droid.BackgroundServices
{
    [Service]
    public class TranscribeItemBackgroundService : Service
    {
        private CancellationTokenSource _cancellationTokenSource;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () => await RunAsync(startId).ConfigureAwait(false), _cancellationTokenSource.Token);

            return StartCommandResult.Sticky;
        }

        public async Task RunAsync(int startId)
        {
            try
            {
                var app = (App)Xamarin.Forms.Application.Current;
                var transcribeItemService = app.Container.Resolve<ITranscribeItemService>();

                await transcribeItemService.AudioSourcesSynchronizationAsync(_cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    StopSelf(startId);
                }
            }
        }

        public override void OnDestroy()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }

            base.OnDestroy();
        }
    }
}