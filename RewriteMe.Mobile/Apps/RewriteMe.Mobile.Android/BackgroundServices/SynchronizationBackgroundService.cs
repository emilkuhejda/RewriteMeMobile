using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Prism.Ioc;
using RewriteMe.Domain.Interfaces.Services;

namespace RewriteMe.Mobile.Droid.BackgroundServices
{
    [Service]
    public class SynchronizationBackgroundService : Service
    {
        private ISchedulerService _schedulerService;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private ISchedulerService SchedulerService
        {
            get
            {
                if (_schedulerService != null)
                    return _schedulerService;

                var app = (App)Xamarin.Forms.Application.Current;
                return _schedulerService = app.Container.Resolve<ISchedulerService>();
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Task.Run(async () => await SchedulerService.Start().ConfigureAwait(false));

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            SchedulerService?.Stop();

            base.OnDestroy();
        }
    }
}