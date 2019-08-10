using System;

namespace RewriteMe.Domain.Configuration
{
    public class AccessToken : AccessTokenBase
    {
        public AccessToken(string accessToken)
        {
            var accessTokenObject = ParseAccessToken(accessToken);

            var exp = accessTokenObject["exp"]?.ToString();

            ExpirationDate = exp == null ? default(DateTimeOffset) : DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));
        }

        public DateTimeOffset ExpirationDate { get; }
    }
}
