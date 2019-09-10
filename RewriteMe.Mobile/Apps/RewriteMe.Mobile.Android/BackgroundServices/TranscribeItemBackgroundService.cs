using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Prism.Ioc;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Interfaces.Services;
using OperationCanceledException = System.OperationCanceledException;

namespace RewriteMe.Mobile.Droid.BackgroundServices
{
    [Service]
    public class TranscribeItemBackgroundService : Service
    {
        private readonly object _lockObject = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            Task.Run(async () => await RunAsync().ConfigureAwait(false), _cancellationTokenSource.Token);

            return StartCommandResult.Sticky;
        }

        public async Task RunAsync()
        {
            lock (_lockObject)
            {
                if (_isRunning)
                    return;

                _isRunning = true;
            }

            try
            {
                var app = (App)Xamarin.Forms.Application.Current;
                var transcribeItemService = app.Container.Resolve<ITranscribeItemService>();

                await transcribeItemService.AudioSourcesSynchronizationAsync(_cancellationTokenSource.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (UnauthorizedCallException)
            {
                _cancellationTokenSource.Cancel();
            }
            finally
            {
                _isRunning = false;
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