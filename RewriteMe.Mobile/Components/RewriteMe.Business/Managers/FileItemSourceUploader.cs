using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Managers;

namespace RewriteMe.Business.Managers
{
    public class FileItemSourceUploader : SynchronizationManager, IFileItemSourceUploader
    {
        private readonly object _lockObject = new object();

        public async Task UploadAsync()
        {
            lock (_lockObject)
            {
                if (IsRunning)
                    return;

                IsRunning = true;
            }

            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();

            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
