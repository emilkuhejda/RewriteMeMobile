using System;
using System.Text;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Business.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// An additional pad character is allocated which may be used to force the encoded output into an integer multiple of 4 characters
        /// (or equivalently when the unencoded binary text is not a multiple of 3 bytes) ; these padding characters must then be discarded
        /// when decoding but still allow the calculation of the effective length of the unencoded text, when its input binary length would
        /// not be a multiple of 3 bytes (the last non-pad character is normally encoded so that the last 6-bit block it represents will be
        /// zero-padded on its least significant bits, at most two pad characters may occur at the end of the encoded stream).
        /// https://stackoverflow.com/questions/4492426/remove-trailing-when-base64-encoding
        /// </summary>
        public static string Base64UrlDecode(this string value)
        {
            value = value.Replace('-', '+').Replace('_', '/');
            value = value.PadRight(value.Length + ((4 - (value.Length % 4)) % 4), '=');
            var byteArray = Convert.FromBase64String(value);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
            return decoded;
        }

        public static string Base64UrlEncode(this string value)
        {
            char[] padding = { '=' };
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes)
                .TrimEnd(padding)
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static RecognitionState ToRecognitionState(this string value)
        {
            return ToEnum(value, RecognitionState.None);
        }

        private static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            return Enum.TryParse(value, true, out T result) ? result : defaultValue;
        }
    }
}
