using RewriteMe.Domain.Enums;

namespace RewriteMe.Domain.Messages
{
    public class StopBackgroundServiceMessage
    {
        public StopBackgroundServiceMessage(BackgroundServiceType serviceType)
        {
            ServiceType = serviceType;
        }

        public BackgroundServiceType ServiceType { get; }
    }
}
