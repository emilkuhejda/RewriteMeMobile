using System.Collections.Generic;

namespace RewriteMe.Domain
{
    public class HeadersDictionary : Dictionary<string, List<string>>
    {
        public HeadersDictionary AddBearerToken(string accessToken)
        {
            Add("Authorization", new List<string> { $"Bearer {accessToken}" });
            return this;
        }
    }
}
