using System;
using Microsoft.Identity.Client;

namespace RewriteMe.Business.Wrappers
{
    /// <summary>
    /// In B2C, currently, the displayName (aka username aka preferred username) is null.
    /// This is a "bug" in B2C as they should provide a scope for the username.
    /// DisplayName should never be null - it would be a schema violation for it to be null.
    /// The Microsoft.Identity.Client Cache will replace an empty Username with "Missing from the token response"
    /// https://github.com/AzureAD/microsoft-authentication-library-for-dotnet/blob/d3e6da0523c178b2e44ca6072b12cf82e4feb23b/src/Microsoft.Identity.Client/Cache/cache.readme.md
    /// 
    /// This class can be used to wrap an IAccount to avoid having "Missing from the token response" as username in the input box 
    /// when navigating e.g. to the edit profile webview
    /// </summary>
    public class EmptyUsernameAccountWrapper : IAccount
    {
        private readonly IAccount _account;

        private EmptyUsernameAccountWrapper(IAccount account)
        {
            _account = account ?? throw new ArgumentNullException(nameof(account));
        }

        public string Username => "Missing from the token response".Equals(_account.Username, StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : _account.Username;

        public string Environment => _account.Environment;

        public AccountId HomeAccountId => _account.HomeAccountId;

        public static IAccount WrapOrNull(IAccount account) => account == null
            ? null
            : new EmptyUsernameAccountWrapper(account);
    }
}
