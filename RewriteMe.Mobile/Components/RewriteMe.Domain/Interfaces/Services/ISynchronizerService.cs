using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Events;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface ISynchronizerService
    {
        event EventHandler<RecognitionErrorOccurredEventArgs> RecognitionErrorOccurred;
        event EventHandler UnauthorizedCallOccurred;

        Task StartAsync();

        void Cancel();
    }
}
