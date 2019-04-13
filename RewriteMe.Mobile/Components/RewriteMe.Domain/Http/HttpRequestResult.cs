namespace RewriteMe.Domain.Http
{
    public class HttpRequestResult<T> where T : class
    {
        public HttpRequestResult(HttpRequestState state)
            : this(state, null, null)
        {
        }

        public HttpRequestResult(HttpRequestState state, int? status)
            : this(state, status, null)
        {
        }

        public HttpRequestResult(HttpRequestState state, int? status, T payload)
        {
            State = state;
            Status = status;
            Payload = payload;
        }

        public HttpRequestState State { get; }

        public int? Status { get; }

        public T Payload { get; }
    }
}
