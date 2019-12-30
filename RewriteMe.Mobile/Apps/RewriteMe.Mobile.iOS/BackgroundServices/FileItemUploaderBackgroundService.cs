using System.Threading.Tasks;
using Prism.Ioc;
using RewriteMe.Domain.Interfaces.Managers;
using UIKit;

namespace RewriteMe.Mobile.iOS.BackgroundServices
{
    public class FileItemUploaderBackgroundService
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

        public async Task RunAsync()
        {
            var taskId = UIApplication.SharedApplication.BeginBackgroundTask(nameof(FileItemUploaderBackgroundService), OnExpiration);

            await FileItemSourceUploader.UploadAsync().ConfigureAwait(false);

            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        }

        public void Stop()
        {
            FileItemSourceUploader?.Cancel();
        }

        private void OnExpiration()
        {
            FileItemSourceUploader?.Cancel();
        }
    }
}