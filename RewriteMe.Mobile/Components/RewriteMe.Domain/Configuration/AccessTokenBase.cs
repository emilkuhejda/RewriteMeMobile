using System;
using Newtonsoft.Json.Linq;
using RewriteMe.Common.Extensions;

namespace RewriteMe.Domain.Configuration
{
    public abstract class AccessTokenBase
    {
        protected JObject ParseAccessToken(string accessToken)
        {
            if (accessToken == null)
                throw new ArgumentNullException(nameof(accessToken));

            var dataPartEncoded = accessToken.Split('.')[1];
            var dataPartDecoded = dataPartEncoded.Base64UrlDecode();
            var accessTokenObject = JObject.Parse(dataPartDecoded);

            return accessTokenObject;
        }
    }
}
