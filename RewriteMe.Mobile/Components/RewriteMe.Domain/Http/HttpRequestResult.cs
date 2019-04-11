namespace RewriteMe.Domain.Http
{
    public class HttpRequestResult<T> where T : class
    {
        public HttpRequestResult(HttpRequestState state)
            : this(state, null)
        {
        }

        public HttpRequestResult(HttpRequestState state, T payload)
        {
            State = state;
            Payload = payload;
        }

        public HttpRequestState State { get; }

        public T Payload { get; }
    }
}
