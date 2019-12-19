using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace RewriteMe.Domain.WebApi
{
    public partial class RewriteMeClient
    {
        private Dictionary<string, List<string>> CustomHeaders { get; set; }

        public void AddCustomHeaders(Dictionary<string, List<string>> customHeaders)
        {
            CustomHeaders = customHeaders;
        }

#pragma warning disable CA1801
        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, string url)
        {
            if (CustomHeaders != null)
            {
                foreach (var header in CustomHeaders)
                {
                    if (request.Headers.Contains(header.Key))
                    {
                        request.Headers.Remove(header.Key);
                    }
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder)
        {
            if (request.Content.Headers.ContentType.MediaType == "multipart/form-data")
            {
                var boundary = System.Guid.NewGuid().ToString();
                var content = new MultipartFormDataContent(boundary);
                content.Headers.Remove("Content-Type");
                content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);

                var contentFile = request.Content;
                content.Add(contentFile, "file", "file");

                request.Content = content;
            }
        }
#pragma warning restore CA1801
    }
}
