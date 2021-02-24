using System;
using System.Collections.Generic;

namespace RewriteMe.Mobile.Transcription
{
    public static class MediaContentTypes
    {
        private static readonly IList<string> Mp3ContentTypes = new List<string>
        {
            "audio/mp3",
            "audio/mpeg",
            "audio/mpeg3",
            "audio/x-mpeg-3",
            "video/mpeg",
            "video/x-mpeg"
        };

        public static bool IsUnsupported(string contentType)
        {
            return !contentType.Contains("audio", StringComparison.InvariantCultureIgnoreCase) || Mp3ContentTypes.Contains(contentType);
        }
    }
}
