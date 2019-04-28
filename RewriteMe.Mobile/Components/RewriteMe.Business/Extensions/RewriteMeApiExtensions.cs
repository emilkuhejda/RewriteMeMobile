using System.Threading.Tasks;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.WebApi;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Extensions
{
    public static class RewriteMeApiExtensions
    {
        public static async Task<Ok> RegisterUserAsync(this IRewriteMeAPI operations, RegisterUserModel registerUserModel)
        {
            using (var result = await operations.RegisterUserWithHttpMessagesAsync(registerUserModel).ConfigureAwait(false))
            {
                return ParseBody<Ok>(result.Body);
            }
        }

        private static T ParseBody<T>(object body) where T : new()
        {
            if (body is ProblemDetails problemDetails)
                throw new ProblemDetailsException(problemDetails);

            return (T)body;
        }
    }
}
