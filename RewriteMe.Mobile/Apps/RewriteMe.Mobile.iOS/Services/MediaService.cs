using System;
using AVFoundation;
using Foundation;
using RewriteMe.Domain.Interfaces.Required;

namespace RewriteMe.Mobile.iOS.Services
{
    public class MediaService : IMediaService
    {
        public TimeSpan GetTotalTime(string fileName)
        {
            var filePath = NSBundle.MainBundle.PathForResource(fileName, null);
            using (var url = new NSUrl(filePath ?? fileName))
            {
                var audioPlayer = AVAudioPlayer.FromUrl(url);
                return TimeSpan.FromSeconds(audioPlayer.Duration);
            }
        }
    }
}