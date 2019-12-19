using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Utils;

namespace RewriteMe.Business.Services
{
    public abstract class WebServiceBase
    {
        protected WebServiceBase(
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
        {
            WebServiceErrorHandler = webServiceErrorHandler;
            ApplicationSettings = applicationSettings;
        }
        
        protected IWebServiceErrorHandler WebServiceErrorHandler { get; }

        protected IApplicationSettings ApplicationSettings { get; }
    }
}
