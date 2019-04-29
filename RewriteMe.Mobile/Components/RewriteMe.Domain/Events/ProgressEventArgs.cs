using System;

namespace RewriteMe.Domain.Events
{
    public class ProgressEventArgs : EventArgs
    {
        public ProgressEventArgs(int totalSteps, int stepsDone)
        {
            TotalSteps = totalSteps;
            StepsDone = stepsDone;
        }

        public int TotalSteps { get; }

        public int StepsDone { get; }

        public int PercentageDone => (int)((double)StepsDone / TotalSteps * 100);
    }
}
