﻿using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRegistrationUserWebService
    {
        Task<HttpRequestResult<UserRegistration>> RegisterUserAsync(RegisterUserModel registerUserModel, string b2CAccessToken);

        Task<HttpRequestResult<User>> UpdateUserAsync(UpdateUserModel updateUserModel, string accessToken);
    }
}
