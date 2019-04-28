using System;
using Newtonsoft.Json.Linq;
using RewriteMe.Business.Extensions;

namespace RewriteMe.Business.Configuration
{
    public class AccessToken
    {
        public AccessToken(string accessToken)
        {
            if (accessToken == null)
                throw new ArgumentNullException(nameof(accessToken));

            var dataPartEncoded = accessToken.Split('.')[1];
            var dataPartDecoded = dataPartEncoded.Base64UrlDecode();
            var accessTokenObject = JObject.Parse(dataPartDecoded);

            ObjectId = accessTokenObject["oid"]?.ToString();
            GivenName = accessTokenObject["given_name"]?.ToString();
            FamilyName = accessTokenObject["family_name"]?.ToString();
            NewUser = accessTokenObject["newUser"] != null && (bool)accessTokenObject["newUser"];
        }

        public string ObjectId { get; }

        public string GivenName { get; }

        public string FamilyName { get; }

        public bool NewUser { get; }
    }
}
