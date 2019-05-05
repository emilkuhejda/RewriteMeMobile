using System;
using System.IO;

namespace RewriteMe.Domain.Transcription
{
    public class MediaFile
    {
        public string Name { get; set; }

        public string Language { get; set; }

        public string FileName { get; set; }

        public TimeSpan TotalTime { get; set; }

        public Stream Stream { get; set; }
    }
}
