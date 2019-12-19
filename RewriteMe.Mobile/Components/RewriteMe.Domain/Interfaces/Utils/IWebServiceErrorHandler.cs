using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Interfaces.Utils
{
    public interface IWebServiceErrorHandler
    {
        Task<HttpRequestResult<T>> HandleResponseAsync<T>(Func<RewriteMeClient, Task<T>> webServiceCall) where T : class;
    }
}
