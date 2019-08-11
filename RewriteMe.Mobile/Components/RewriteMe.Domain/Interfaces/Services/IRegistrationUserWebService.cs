using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRegistrationUserWebService
    {
        Task<HttpRequestResult<RegistrationModel>> RegisterUserAsync(RegisterUserModel registerUserModel, string accessToken);
    }
}
