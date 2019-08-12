using System;
using Newtonsoft.Json.Linq;
using RewriteMe.Common.Extensions;

namespace RewriteMe.Domain.Configuration
{
    public abstract class AccessTokenBase
    {
        protected AccessTokenBase(string accessToken)
        {
            Token = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        }

        public string Token { get; }

        protected JObject ParseAccessToken()
        {
            var dataPartEncoded = Token.Split('.')[1];
            var dataPartDecoded = dataPartEncoded.Base64UrlDecode();
            var accessTokenObject = JObject.Parse(dataPartDecoded);

            return accessTokenObject;
        }
    }
}
