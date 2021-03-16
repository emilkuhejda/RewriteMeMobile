using System;

namespace RewriteMe.Mobile.Transcription
{
    public static class MediaContentTypes
    {
        public static bool IsUnsupported(string contentType)
        {
            return !contentType.Contains("audio", StringComparison.OrdinalIgnoreCase);
        }
    }
}
