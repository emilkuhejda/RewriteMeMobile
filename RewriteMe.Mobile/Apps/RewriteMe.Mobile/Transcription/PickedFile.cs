namespace RewriteMe.Mobile.Transcription
{
    public class PickedFile
    {
        public string FileName { get; set; }

        public bool CanTranscribe { get; set; }

        public byte[] Source { get; set; }
    }
}
