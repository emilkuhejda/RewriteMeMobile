using System;
using System.Collections.Generic;

namespace RewriteMe.Mobile.Transcription
{
    public static class MediaContentTypes
    {
        private static readonly HashSet<string> Mp3ContentTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "video/mpeg",
            "video/x-mpeg"
        };

        public static bool IsUnsupported(string contentType)
        {
            return !contentType.Contains("audio", StringComparison.OrdinalIgnoreCase) || Mp3ContentTypes.Contains(contentType);
        }
    }
}
