using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRewriteMeWebService
    {
        Task<HttpRequestResult<ProblemDetails>> RegisterUserAsync(RegisterUserModel registerUserModel);
    }
}
