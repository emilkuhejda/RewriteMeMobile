using Prism.Navigation;

namespace RewriteMe.Mobile.Extensions
{
    public static class NavigationParameterExtensions
    {
        public static void Add<T>(this NavigationParameters navigationParameters, object value)
        {
            navigationParameters.Add(typeof(T).Name, value);
        }

        public static T GetValue<T>(this INavigationParameters navigationParameters)
        {
            return navigationParameters.GetValue<T>(typeof(T).Name);
        }
    }
}
