using System;
using Android.Media;
using Plugin.CurrentActivity;
using RewriteMe.Domain.Interfaces.Required;
using Uri = Android.Net.Uri;

namespace RewriteMe.Mobile.Droid.Services
{
    public class MediaService : IMediaService
    {
        public TimeSpan GetDuration(string fileName)
        {
            using (var mediaPlayer = MediaPlayer.Create(CrossCurrentActivity.Current.AppContext, Uri.Parse(fileName)))
            {
                return TimeSpan.FromMilliseconds(mediaPlayer.Duration);
            }
        }
    }
}