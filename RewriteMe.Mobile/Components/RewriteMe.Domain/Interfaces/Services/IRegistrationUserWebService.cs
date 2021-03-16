using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRegistrationUserWebService
    {
        Task<HttpRequestResult<UserRegistration>> RegisterUserAsync(UserRegistrationInputModel registrationUserModel, string b2CAccessToken);

        Task<HttpRequestResult<Identity>> UpdateUserAsync(UpdateUserInputModel updateUserModel, string accessToken);
    }
}
