using Prism.Ioc;
using RewriteMe.Mobile.ViewModels;

namespace RewriteMe.Mobile.Configuration
{
    public static class Locator
    {
        private static IContainerProvider _containerProvider;

        public static void SetContainerProvider(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
        }

        public static BottomNavigationViewModel BottomNavigation => _containerProvider.Resolve<BottomNavigationViewModel>();
    }
}
