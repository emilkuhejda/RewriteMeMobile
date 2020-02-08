using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Extensions
{
    public static class RewriteMeClientExtensions
    {
        public static async Task<Ok> UploadChunkFileAsync(this RewriteMeClient rewriteMeClient, Guid fileItemId, int order, StorageSetting storageSetting, Guid applicationId, string version, byte[] source, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream(source))
            {
                return await rewriteMeClient.UploadChunkFileAsync(fileItemId, order, storageSetting, applicationId, version, stream, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
