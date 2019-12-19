using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Rest;
using RewriteMe.Business.Extensions;
using RewriteMe.Common.Utils;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Interfaces.Utils;
using RewriteMe.Domain.WebApi;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Utils
{
    public class WebServiceErrorHandler : IWebServiceErrorHandler
    {
        private readonly IConnectivityService _connectivityService;
        private readonly ILogger _logger;

        public WebServiceErrorHandler(
            IConnectivityService connectivityService,
            ILoggerFactory loggerFactory)
        {
            _connectivityService = connectivityService;
            _logger = loggerFactory.CreateLogger(typeof(IWebServiceErrorHandler));
        }

        public async Task<HttpRequestResult<T>> HandleResponseAsync<T>(Func<Task<T>> webServiceCall) where T : class
        {
            if (webServiceCall == null)
                throw new ArgumentNullException(nameof(webServiceCall));

            if (!_connectivityService.IsConnected)
                return new HttpRequestResult<T>(HttpRequestState.Offline);

            var targetTypeName = typeof(T).GetFriendlyName();
            try
            {
                _logger.Info($"Web service request for type '{targetTypeName}' started.");
                var payload = await webServiceCall().ConfigureAwait(false);
                _logger.Info($"Web service request for type '{targetTypeName}' finished.");
                return new HttpRequestResult<T>(HttpRequestState.Success, (int)HttpStatusCode.OK, payload);
            }
            catch (ApiException exception)
            {
                _logger.Warning($"Web service request for type '{targetTypeName}' finished with error status code: '{exception.StatusCode}'.");
                _logger.Error(ExceptionFormatter.FormatException(exception));
                if (exception.StatusCode == (int)HttpStatusCode.Unauthorized)
                    throw new UnauthorizedCallException();

                return new HttpRequestResult<T>(HttpRequestState.Error, exception.StatusCode);
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
            catch (OperationCanceledException exception)
            {
                var message = $"Operation canceled during '{targetTypeName}' web service request.";
                _logger.Warning($"{message} {exception}");

                return new HttpRequestResult<T>(HttpRequestState.Canceled);
            }
        }
    }
}
