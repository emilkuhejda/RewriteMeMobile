using System;
using Microsoft.Identity.Client;
using RewriteMe.Domain.Interfaces.Factories;

namespace RewriteMe.Business.Factories
{
    public class PublicClientApplicationFactory : IPublicClientApplicationFactory
    {
        public IPublicClientApplication CreatePublicClientApplication(string clienId, string redirectUri)
        {
            if (clienId == null)
                throw new ArgumentNullException(nameof(clienId));
            if (redirectUri == null)
                throw new ArgumentNullException(nameof(redirectUri));

            return new PublicClientApplication(clienId)
            {
                RedirectUri = redirectUri
            };
        }
    }
}
