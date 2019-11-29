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
            apiClient.Optimize(Timeout);
        }

        public static void Optimize(this IApiClient apiClient, TimeSpan timeout)
        {
            if (apiClient == null)
                throw new ArgumentNullException(nameof(apiClient));

            apiClient.HttpClient.Timeout = timeout;
            apiClient.HttpHandler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            apiClient.HttpHandler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
        }
    }
}
