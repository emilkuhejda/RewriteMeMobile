using System.Collections.Generic;

namespace RewriteMe.Domain.Http
{
    public class CustomHeadersDictionary : Dictionary<string, List<string>>
    {
        public CustomHeadersDictionary AddBearerToken(string accessToken)
        {
            Add("Authorization", new List<string> { $"Bearer {accessToken}" });
            return this;
        }
    }
}
