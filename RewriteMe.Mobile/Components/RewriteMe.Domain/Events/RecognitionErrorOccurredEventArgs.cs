using System;

namespace RewriteMe.Domain.Events
{
    public class RecognitionErrorOccurredEventArgs : EventArgs
    {
        public RecognitionErrorOccurredEventArgs(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }
    }
}
