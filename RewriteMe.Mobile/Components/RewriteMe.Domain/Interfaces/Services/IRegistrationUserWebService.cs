using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRegistrationUserWebService
    {
        Task<HttpRequestResult<UserRegistration>> RegisterUserAsync(RegistrationUserModel registrationUserModel, string b2CAccessToken);

        Task<HttpRequestResult<Identity>> UpdateUserAsync(UpdateUserModel updateUserModel, string accessToken);
    }
}
