using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.Business.Extensions
{
    public static class RewriteMeClientExtensions
    {
        public static async Task<FileItem> UploadSourceFileAsync(this RewriteMeClient rewriteMeClient, Guid fileItemId, Guid applicationId, string version, byte[] source, CancellationToken cancellationToken)
        {
            using (var stream = new MemoryStream(source))
            {
                return await rewriteMeClient.UploadSourceFileAsync(fileItemId, applicationId, version, stream, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
