﻿using System.Collections.Generic;
using System.Net.Http;

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
#pragma warning restore CA1801
    }
}
