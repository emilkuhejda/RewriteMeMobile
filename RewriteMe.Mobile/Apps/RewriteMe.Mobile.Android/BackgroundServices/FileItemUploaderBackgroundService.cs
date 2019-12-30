using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Prism.Ioc;
using RewriteMe.Domain.Interfaces.Managers;

namespace RewriteMe.Mobile.Droid.BackgroundServices
{
    [Service]
    public class FileItemUploaderBackgroundService : Service
    {
        private IFileItemSourceUploader _fileItemSourceUploader;

        private IFileItemSourceUploader FileItemSourceUploader
        {
            get
            {
                if (_fileItemSourceUploader != null)
                    return _fileItemSourceUploader;

                var app = (App)Xamarin.Forms.Application.Current;
                return _fileItemSourceUploader = app.Container.Resolve<IFileItemSourceUploader>();
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Task.Run(() => FileItemSourceUploader.UploadAsync()).ConfigureAwait(false);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            FileItemSourceUploader?.Cancel();

            base.OnDestroy();
        }
    }
}