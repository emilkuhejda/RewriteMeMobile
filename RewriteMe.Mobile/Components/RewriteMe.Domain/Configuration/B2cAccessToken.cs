using Newtonsoft.Json;

namespace RewriteMe.Domain.Configuration
{
    public class B2CAccessToken : AccessTokenBase
    {
        public B2CAccessToken(string accessToken)
        {
            var accessTokenObject = ParseAccessToken(accessToken);

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
