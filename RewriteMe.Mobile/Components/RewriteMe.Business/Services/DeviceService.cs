using RewriteMe.Domain.Interfaces.Services;
using Xamarin.Forms;

namespace RewriteMe.Business.Services
{
    public class DeviceService : IDeviceService
    {
        public string RuntimePlatform => Device.RuntimePlatform;
    }
}
