using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Domain.Enums;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface INavigator
    {
        RootPage CurrentPage { get; }

        Task NavigateToAsync(string name, RootPage rootPage, INavigationParameters navigationParameters = null);
    }
}
