using Plugin.CurrentActivity;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.Droid.Providers
{
    public class IdentityUiParentProvider : IIdentityUiParentProvider
    {
        public object GetUiParent()
        {
            return CrossCurrentActivity.Current.Activity;
        }
    }
}