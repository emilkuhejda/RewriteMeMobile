using System.IO;

namespace RewriteMe.Domain.Transcription
{
    public class MediaFile
    {
        public string Name { get; set; }

        public string Language { get; set; }

        public Stream Stream { get; set; }
    }
}
