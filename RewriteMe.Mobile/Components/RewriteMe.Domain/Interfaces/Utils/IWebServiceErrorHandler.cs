using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;

namespace RewriteMe.Domain.Interfaces.Utils
{
    public interface IWebServiceErrorHandler
    {
        Task<HttpRequestResult<T>> HandleResponseAsync<T>(Func<Task<T>> webServiceCall) where T : class;
    }
}
