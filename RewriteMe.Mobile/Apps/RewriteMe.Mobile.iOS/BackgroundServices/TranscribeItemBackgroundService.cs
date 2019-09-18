using System.Threading.Tasks;
using Prism.Ioc;
using RewriteMe.Domain.Interfaces.Managers;
using UIKit;

namespace RewriteMe.Mobile.iOS.BackgroundServices
{
    public class TranscribeItemBackgroundService
    {
        private ITranscribeItemManager _transcribeItemManager;

        private ITranscribeItemManager TranscribeItemManager
        {
            get
            {
                if (_transcribeItemManager == null)
                {
                    var app = (App)Xamarin.Forms.Application.Current;
                    _transcribeItemManager = app.Container.Resolve<ITranscribeItemManager>();
                }

                return _transcribeItemManager;
            }
        }

        public async Task RunAsync()
        {
            var taskId = UIApplication.SharedApplication.BeginBackgroundTask(nameof(TranscribeItemBackgroundService), OnExpiration);

            await TranscribeItemManager.SynchronizationAsync().ConfigureAwait(false);

            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        }

        public void Stop()
        {
            TranscribeItemManager?.Cancel();
        }

        private void OnExpiration()
        {
            TranscribeItemManager?.Cancel();
        }
    }
}