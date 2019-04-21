using Microsoft.Identity.Client;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.iOS.Providers
{
    public class IdentityUiParentProvider : IIdentityUiParentProvider
    {
        public UIParent GetUiParent()
        {
            return new UIParent(true);
        }
    }
}
