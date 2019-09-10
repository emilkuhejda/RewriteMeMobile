using RewriteMe.Domain.Enums;

namespace RewriteMe.Domain.Messages
{
    public class TranscribeItemBackgroundServiceStatusChangedMessage
    {
        public TranscribeItemBackgroundServiceStatusChangedMessage(RunningStatus status)
        {
            Status = status;
        }

        public RunningStatus Status { get; }
    }
}
