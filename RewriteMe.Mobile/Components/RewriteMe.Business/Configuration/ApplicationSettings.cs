using RewriteMe.Domain.Interfaces.Configuration;

namespace RewriteMe.Business.Configuration
{
    public class ApplicationSettings : IApplicationSettings
    {
        public string SupportMailAddress => "emil.kuhejda@gmail.com";
    }
}
