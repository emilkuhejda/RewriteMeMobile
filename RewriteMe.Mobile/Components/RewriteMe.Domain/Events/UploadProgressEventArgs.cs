using System;

namespace RewriteMe.Domain.Events
{
    public class UploadProgressEventArgs : ProgressEventArgs
    {
        public UploadProgressEventArgs(Guid fileItemId, int totalSteps, int stepsDone)
            : base(totalSteps, stepsDone)
        {
            FileItemId = fileItemId;
        }

        public Guid FileItemId { get; }
    }
}
