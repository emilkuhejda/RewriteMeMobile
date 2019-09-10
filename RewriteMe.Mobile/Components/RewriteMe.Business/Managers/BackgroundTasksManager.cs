using RewriteMe.Domain.Interfaces.Managers;
using RewriteMe.Domain.Messages;
using Xamarin.Forms;

namespace RewriteMe.Business.Managers
{
    public class BackgroundTasksManager : IBackgroundTasksManager
    {
        private readonly IMessagingCenter _messagingCenter;

        public BackgroundTasksManager(IMessagingCenter messagingCenter)
        {
            _messagingCenter = messagingCenter;
        }

        public void Initialize()
        {
            RegisterMessages();
        }

        private void RegisterMessages()
        {
            _messagingCenter.Subscribe<TranscribeItemBackgroundServiceStatusChangedMessage>(
                this,
                nameof(TranscribeItemBackgroundServiceStatusChangedMessage),
                HandleTranscribeItemBackgroundServiceStatusChangedMessage);
        }

        private void HandleTranscribeItemBackgroundServiceStatusChangedMessage(TranscribeItemBackgroundServiceStatusChangedMessage message)
        {
            var status = message.Status;
        }
    }
}
