using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RewriteMe.Business.Extensions;

namespace RewriteMe.Business.Configuration
{
    public class B2CAccessToken
    {
        public B2CAccessToken(string accessToken)
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

            var serializedEmails = accessTokenObject["emails"];
            if (serializedEmails != null)
            {
                Email = JsonConvert.DeserializeObject<string[]>(serializedEmails.ToString())[0];
            }
        }

        public string ObjectId { get; }

        public string Email { get; }

        public string GivenName { get; }

        public string FamilyName { get; }

        public bool NewUser { get; }
    }
}
