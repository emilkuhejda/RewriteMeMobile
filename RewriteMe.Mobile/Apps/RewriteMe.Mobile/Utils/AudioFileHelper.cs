using System;
using System.IO;
using Plugin.SimpleAudioPlayer;

namespace RewriteMe.Mobile.Utils
{
    public static class AudioFileHelper
    {
        public static TimeSpan GetDuration(byte[] source)
        {
            using (var player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer())
            {
                using (var memoryStream = new MemoryStream(source))
                {
                    player.Load(memoryStream);
                    return TimeSpan.FromSeconds(player.Duration);
                }
            }
        }
    }
}
