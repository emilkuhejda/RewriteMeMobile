using System.Threading.Tasks;

namespace RewriteMe.Mobile.iOS.BackgroundServices
{
    public class SynchronizerBackgroundService
    {
        public async Task RunAsync()
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}