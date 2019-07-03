using System;
using System.Threading.Tasks;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRecorderService
    {
        event EventHandler<AudioTranscribedEventArgs> AudioTranscribed;

        event EventHandler StatusChanged;

        TimeSpan Time { get; }

        bool IsRecording { get; }

        Task<RecordedItem> CreateFileAsync();

        bool CanStartRecording();

        bool CanResumeRecording();

        void StartRecording(RecordedItem recordedItem, string subscriptionKey);

        void ResumeRecording();

        void StopRecording();
    }
}
