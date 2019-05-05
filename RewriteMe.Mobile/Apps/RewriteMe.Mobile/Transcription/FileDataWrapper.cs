using System;
using Plugin.FilePicker.Abstractions;

namespace RewriteMe.Mobile.Transcription
{
    public class FileDataWrapper
    {
        public FileDataWrapper(FileData fileData)
        {
            FileData = fileData;
        }

        public FileData FileData { get; }

        public string FileName => FileData.FileName;

        public TimeSpan TotalTime { get; set; }

        public bool CanTranscribe { get; set; }
    }
}
