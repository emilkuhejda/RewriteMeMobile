using System.Threading.Tasks;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IPushNotificationsService
    {
        Task<bool> IsEnabledAsync();

        Task SetEnabledAsync(bool enabled);
    }
}
