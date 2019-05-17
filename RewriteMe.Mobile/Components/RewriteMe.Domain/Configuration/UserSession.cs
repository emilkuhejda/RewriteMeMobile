using System;

namespace RewriteMe.Domain.Configuration
{
    public class UserSession
    {
        public Guid ObjectId { get; set; }

        public string Email { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }
    }
}
