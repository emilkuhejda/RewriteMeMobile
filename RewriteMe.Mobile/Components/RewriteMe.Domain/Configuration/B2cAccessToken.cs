using Newtonsoft.Json;

namespace RewriteMe.Domain.Configuration
{
    public class B2CAccessToken : AccessTokenBase
    {
        public B2CAccessToken(string accessToken)
            : base(accessToken)
        {
            var accessTokenObject = ParseAccessToken();

            ObjectId = accessTokenObject["oid"]?.ToString();
            GivenName = accessTokenObject["given_name"]?.ToString() ?? string.Empty;
            FamilyName = accessTokenObject["family_name"]?.ToString() ?? string.Empty;
            NewUser = accessTokenObject["newUser"] != null && (bool)accessTokenObject["newUser"];

            var serializedEmails = accessTokenObject["emails"];
            if (serializedEmails != null)
            {
                Email = JsonConvert.DeserializeObject<string[]>(serializedEmails.ToString())[0];
            }

            if (string.IsNullOrWhiteSpace(GivenName) && string.IsNullOrWhiteSpace(FamilyName))
            {
                FamilyName = Email.Split('@')[0];
            }
        }

        public string ObjectId { get; }

        public string Email { get; }

        public string GivenName { get; }

        public string FamilyName { get; }

        public bool NewUser { get; }
    }
}
