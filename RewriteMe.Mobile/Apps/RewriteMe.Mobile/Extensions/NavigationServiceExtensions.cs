using System.Threading.Tasks;
using Prism.Navigation;
using RewriteMe.Mobile.Utils;

namespace RewriteMe.Mobile.Extensions
{
    public static class NavigationServiceExtensions
    {
        public static async Task NavigateWithoutAnimationAsync(this INavigationService navigationService, string name, NavigationParameters parameters = null)
        {
            await ThreadHelper.InvokeOnUiThread(() => navigationService.NavigateAsync(name, parameters, null, false).ConfigureAwait(false));
        }

        public static async Task GoBackWithoutAnimationAsync(this INavigationService navigationService, NavigationParameters parameters = null)
        {
            await ThreadHelper.InvokeOnUiThread(() => navigationService.GoBackAsync(parameters, null, false).ConfigureAwait(false));
        }
    }
}
