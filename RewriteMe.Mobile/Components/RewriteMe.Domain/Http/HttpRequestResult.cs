namespace RewriteMe.Domain.Http
{
    public class HttpRequestResult<T> where T : class
    {
        public HttpRequestResult(HttpRequestState state)
            : this(state, null, null)
        {
        }

        public HttpRequestResult(HttpRequestState state, int? statusCode)
            : this(state, statusCode, null)
        {
        }

        public HttpRequestResult(HttpRequestState state, int? statusCode, T payload)
        {
            State = state;
            StatusCode = statusCode;
            Payload = payload;
        }

        public HttpRequestState State { get; }

        public int? StatusCode { get; }

        public T Payload { get; }
    }
}
