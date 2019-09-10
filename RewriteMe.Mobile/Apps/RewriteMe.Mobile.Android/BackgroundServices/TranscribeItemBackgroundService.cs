using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Prism.Ioc;
using RewriteMe.Domain.Interfaces.Managers;

namespace RewriteMe.Mobile.Droid.BackgroundServices
{
    [Service]
    public class TranscribeItemBackgroundService : Service
    {
        private ITranscribeItemManager _transcribeItemManager;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

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

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Task.Run(async () => await TranscribeItemManager.SynchronizationAsync().ConfigureAwait(false));

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            TranscribeItemManager?.Cancel();

            base.OnDestroy();
        }
    }
}