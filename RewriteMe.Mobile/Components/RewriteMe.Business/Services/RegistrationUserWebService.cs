﻿using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Configuration;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Services
{
    public class RegistrationUserWebService : WebServiceBase, IRegistrationUserWebService
    {
        public RegistrationUserWebService(
            IWebServiceErrorHandler webServiceErrorHandler,
            IApplicationSettings applicationSettings)
            : base(webServiceErrorHandler, applicationSettings)
        {
        }

        public async Task<HttpRequestResult<UserRegistration>> RegisterUserAsync(UserRegistrationInputModel registrationUserModel, string b2CAccessToken)
        {
            var customHeaders = new CustomHeadersDictionary().AddBearerToken(b2CAccessToken);
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.RegisterUserAsync(ApplicationSettings.WebApiVersion, registrationUserModel), customHeaders)
                ).ConfigureAwait(false);
        }

        public async Task<HttpRequestResult<Identity>> UpdateUserAsync(UpdateUserInputModel updateUserModel, string accessToken)
        {
            var customHeaders = new CustomHeadersDictionary().AddBearerToken(accessToken);
            return await WebServiceErrorHandler.HandleResponseAsync(
                () => MakeServiceCall(client => client.UpdateUserAsync(ApplicationSettings.WebApiVersion, updateUserModel), customHeaders)
                ).ConfigureAwait(false);
        }
    }
}
