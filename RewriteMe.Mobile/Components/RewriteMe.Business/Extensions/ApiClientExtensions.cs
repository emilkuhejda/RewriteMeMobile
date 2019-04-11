using System;
using System.Net;
using RewriteMe.Domain.Interfaces.Factories;

namespace RewriteMe.Business.Extensions
{
    public static class ApiClientExtensions
    {
        private static TimeSpan Timeout { get; } = TimeSpan.FromMinutes(10);

        public static void Optimize(this IApiClient apiClient)
        {
            if (apiClient == null)
                throw new ArgumentNullException(nameof(apiClient));

            apiClient.HttpClient.Timeout = Timeout;
            apiClient.HttpHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        }
    }
}
