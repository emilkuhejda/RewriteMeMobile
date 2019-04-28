using Microsoft.Identity.Client;
using Plugin.CurrentActivity;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.Droid.Providers
{
    public class IdentityUiParentProvider : IIdentityUiParentProvider
    {
        public UIParent GetUiParent()
        {
            var useEmbeddedWebview = true;
            try
            {
                useEmbeddedWebview = !UIParent.IsSystemWebviewAvailable();
            }
            catch
            {
                // IsSystemWebviewAvailable throws a NameNotFoundException, when chrome is not found, instead of returning false. (2.0.0-preview)
            }

            var activity = CrossCurrentActivity.Current.Activity;
            return new UIParent(activity, useEmbeddedWebview);
        }
    }
}