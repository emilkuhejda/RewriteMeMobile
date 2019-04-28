using Microsoft.Identity.Client;

namespace RewriteMe.Domain.Interfaces.Required
{
    public interface IIdentityUiParentProvider
    {
        UIParent GetUiParent();
    }
}
