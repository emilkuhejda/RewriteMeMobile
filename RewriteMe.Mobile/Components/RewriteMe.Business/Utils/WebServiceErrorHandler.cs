using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Rest;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Utils
{
    public class WebServiceErrorHandler : IWebServiceErrorHandler
    {
        private readonly ILogger _logger;

        public WebServiceErrorHandler(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(typeof(IWebServiceErrorHandler));
        }

        public async Task<HttpRequestResult<T>> HandleResponseAsync<T>(Func<Task<T>> webServiceCall) where T : class
        {
            if (webServiceCall == null)
                throw new ArgumentNullException(nameof(webServiceCall));

            var targetTypeName = typeof(T).GetFriendlyName();
            try
            {
                _logger.Info($"Web service request for type '{targetTypeName}' started.");
                var payload = await webServiceCall().ConfigureAwait(false);
                if (payload is ProblemDetails problemDetails)
                {
                    _logger.Warning($"Web service request for type '{targetTypeName}' finished with error status code: '{problemDetails.Status}'.");
                    return new HttpRequestResult<T>(HttpRequestState.Error, problemDetails.Status);
                }

                _logger.Info($"Web service request for type '{targetTypeName}' finished.");
                return new HttpRequestResult<T>(HttpRequestState.Success, (int)HttpStatusCode.OK, payload);
            }
            catch (HttpRequestException exception)
            {
                var message = $"Request exception during '{targetTypeName}' web service request.";
                _logger.Warning($"{message} {exception}");

                return new HttpRequestResult<T>(HttpRequestState.Offline);
            }
            catch (HttpOperationException exception)
            {
                var message = $"Operation exception during '{targetTypeName}' web service request.";
                _logger.Warning($"{message} {exception}");

                return new HttpRequestResult<T>(HttpRequestState.Offline);
            }
            catch (SerializationException exception)
            {
                var message = $"Serialization exception during '{targetTypeName}' web service request.";
                _logger.Warning($"{message} {exception}");

                return new HttpRequestResult<T>(HttpRequestState.Offline);
            }
            catch (TaskCanceledException exception)
            {
                var message = $"Timeout exception during '{targetTypeName}' web service request.";
                _logger.Warning($"{message} {exception}");

                return new HttpRequestResult<T>(HttpRequestState.Offline);
            }
        }
    }
}
