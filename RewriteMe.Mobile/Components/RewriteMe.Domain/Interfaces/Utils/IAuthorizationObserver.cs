using System.Threading.Tasks;
using Prism.Navigation;

namespace RewriteMe.Domain.Interfaces.Utils
{
    public interface IAuthorizationObserver
    {
        void Initialize(INavigationService navigationService);

        Task LogOutAsync();
    }
}
