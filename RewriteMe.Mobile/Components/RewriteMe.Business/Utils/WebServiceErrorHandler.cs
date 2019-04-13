using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Rest;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Utils
{
    public class WebServiceErrorHandler : IWebServiceErrorHandler
    {
        public async Task<HttpRequestResult<T>> HandleResponseAsync<T>(Func<Task<T>> webServiceCall) where T : class
        {
            if (webServiceCall == null)
                throw new ArgumentNullException(nameof(webServiceCall));

            try
            {
                var payload = await webServiceCall().ConfigureAwait(false);
                if (payload is ProblemDetails problemDetails)
                    return new HttpRequestResult<T>(HttpRequestState.Success, problemDetails.Status);

                return new HttpRequestResult<T>(HttpRequestState.Success, (int)HttpStatusCode.OK, payload);
            }
            catch (HttpRequestException)
            {
                return new HttpRequestResult<T>(HttpRequestState.Offline);
            }
            catch (HttpOperationException)
            {
                return new HttpRequestResult<T>(HttpRequestState.Offline);
            }
            catch (SerializationException)
            {
                return new HttpRequestResult<T>(HttpRequestState.Offline);
            }
            catch (TaskCanceledException)
            {
                return new HttpRequestResult<T>(HttpRequestState.Offline);
            }
        }
    }
}
