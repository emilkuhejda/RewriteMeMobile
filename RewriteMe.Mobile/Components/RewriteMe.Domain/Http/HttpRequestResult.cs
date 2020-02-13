using RewriteMe.Domain.WebApi;

namespace RewriteMe.Domain.Http
{
    public class HttpRequestResult<T> where T : class
    {
        public HttpRequestResult(HttpRequestState state)
            : this(state, null)
        {
        }

        public HttpRequestResult(HttpRequestState state, int? statusCode)
            : this(state, statusCode, null)
        {
        }

        public HttpRequestResult(HttpRequestState state, int? statusCode, T payload)
        : this(state, statusCode, payload, ErrorCode.None)
        { }

        public HttpRequestResult(HttpRequestState state, int? statusCode, ErrorCode errorCode)
            : this(state, statusCode, null, errorCode)
        { }

        public HttpRequestResult(HttpRequestState state, int? statusCode, T payload, ErrorCode errorCode)
        {
            State = state;
            StatusCode = statusCode;
            Payload = payload;
            ErrorCode = errorCode;
        }

        public HttpRequestState State { get; }

        public int? StatusCode { get; }

        public T Payload { get; }

        public ErrorCode ErrorCode { get; }
    }
}
