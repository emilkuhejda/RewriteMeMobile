using System;

namespace RewriteMe.Domain.Upload
{
    public class UploadedSource
    {
        public Guid Id { get; set; }

        public Guid FileItemId { get; set; }

        public byte[] Source { get; set; }

        public bool IsTranscript { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
