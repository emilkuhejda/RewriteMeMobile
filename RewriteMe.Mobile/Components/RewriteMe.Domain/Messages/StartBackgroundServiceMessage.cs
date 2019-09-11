using RewriteMe.Domain.Enums;

namespace RewriteMe.Domain.Messages
{
    public class StartBackgroundServiceMessage
    {
        public StartBackgroundServiceMessage(BackgroundServiceType serviceType)
        {
            ServiceType = serviceType;
        }

        public BackgroundServiceType ServiceType { get; }
    }
}
