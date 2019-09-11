using System.Threading.Tasks;
using Prism.Ioc;
using RewriteMe.Domain.Interfaces.Services;
using UIKit;

namespace RewriteMe.Mobile.iOS.BackgroundServices
{
    public class SynchronizerBackgroundService
    {
        private ISynchronizerService _synchronizerService;

        private ISynchronizerService SynchronizerService
        {
            get
            {
                if (_synchronizerService != null)
                    return _synchronizerService;

                var app = (App)Xamarin.Forms.Application.Current;
                return _synchronizerService = app.Container.Resolve<ISynchronizerService>();
            }
        }

        public async Task RunAsync()
        {
            var taskId = UIApplication.SharedApplication.BeginBackgroundTask(nameof(SynchronizerBackgroundService), OnExpiration);

            await SynchronizerService.StartAsync().ConfigureAwait(false);

            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        }

        private void OnExpiration()
        {
            SynchronizerService?.Cancel();
        }
    }
}