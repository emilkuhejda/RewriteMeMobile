using Microsoft.Identity.Client;

namespace RewriteMe.Domain.Interfaces.Factories
{
    public interface IPublicClientApplicationFactory
    {
        IPublicClientApplication CreatePublicClientApplication(string clienId, string redirectUri);
    }
}
