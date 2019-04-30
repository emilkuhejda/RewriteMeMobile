namespace RewriteMe.Domain.Transcription
{
    public enum RecognitionState
    {
        None = 0,
        Converting,
        Prepared,
        InProgress,
        Completed
    }
}
