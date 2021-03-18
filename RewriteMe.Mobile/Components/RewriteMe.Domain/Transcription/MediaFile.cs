namespace RewriteMe.Domain.Transcription
{
    public class MediaFile
    {
        public string Name { get; set; }

        public string Language { get; set; }

        public string FileName { get; set; }

        public bool IsPhoneCall { get; set; }

        public byte[] Source { get; set; }
    }
}
