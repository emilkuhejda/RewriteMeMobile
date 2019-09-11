using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Prism.Ioc;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Mobile.Droid.BackgroundServices
{
    [Service]
    public class SynchronizerBackgroundService : Service
    {
        private ISynchronizerService _synchronizerService;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

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

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Task.Run(async () => await SynchronizerService.Start().ConfigureAwait(false));

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            SynchronizerService?.Stop();

            base.OnDestroy();
        }
    }
}