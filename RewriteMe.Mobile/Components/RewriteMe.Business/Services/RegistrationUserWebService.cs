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

        public async Task<HttpRequestResult<UserRegistration>> RegisterUserAsync(RegisterUserModel registerUserModel, string b2CAccessToken)
        {
            registerUserModel.ApplicationId = ApplicationSettings.ApplicationId;
            var customHeaders = new CustomHeadersDictionary().AddBearerToken(b2CAccessToken);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.RegisterUserAsync(registerUserModel, customHeaders)).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Identity>> UpdateUserAsync(UpdateUserModel updateUserModel, string accessToken)
        {
            var customHeaders = new CustomHeadersDictionary().AddBearerToken(accessToken);
            return await WebServiceErrorHandler.HandleResponseAsync(() => Client.UpdateUserAsync(updateUserModel, customHeaders)).ConfigureAwait(false);
        }
    }
}
