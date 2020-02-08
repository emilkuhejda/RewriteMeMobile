using System;

namespace RewriteMe.Domain.Upload
{
    public class UploadedFile
    {
        public UploadedFile(Guid fileItemId)
        {
            FileItemId = fileItemId;
        }

        public Guid FileItemId { get; }

        public int Progress { get; set; }
    }
}
