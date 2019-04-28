using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Factories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Services
{
    public class RegistrationUserWebService : WebServiceBase, IRegistrationUserWebService
    {
        public RegistrationUserWebService(
            IRewriteMeApiClientFactory rewriteMeApiClientFactory,
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
            : base(rewriteMeApiClientFactory, webServiceErrorHandler, applicationSettings)
        {
        }

        public async Task<HttpRequestResult<Ok>> RegisterUserAsync(RegisterUserModel registerUserModel)
        {
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.RegisterUserAsync(registerUserModel));
        }
    }
}
