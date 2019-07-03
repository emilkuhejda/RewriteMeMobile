using System;

namespace RewriteMe.Domain.Events
{
    public class AudioTranscribedEventArgs : EventArgs
    {
        public AudioTranscribedEventArgs(string transcript)
        {
            Transcript = transcript;
        }

        public string Transcript { get; }
    }
}
