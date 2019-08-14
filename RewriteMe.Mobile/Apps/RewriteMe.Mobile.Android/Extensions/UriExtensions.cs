using System;
using Android.Content;
using Uri = Android.Net.Uri;

namespace RewriteMe.Mobile.Droid.Extensions
{
    public static class UriExtensions
    {
        public static string GetPath(this Uri uri, ContentResolver contentResolver)
        {
            using (var cursor = contentResolver.Query(uri, null, null, null))
            {
                cursor.MoveToFirst();
                var document = cursor.GetString(0);
                var index = document.LastIndexOf(":", StringComparison.Ordinal);
                return document.Substring(index + 1);
            }
        }
    }
}
