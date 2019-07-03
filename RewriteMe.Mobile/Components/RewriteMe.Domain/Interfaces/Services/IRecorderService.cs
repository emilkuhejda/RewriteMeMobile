using System;
using System.Threading.Tasks;
using System.Timers;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRecorderService
    {
        event EventHandler<AudioTranscribedEventArgs> AudioTranscribed;

        event EventHandler StatusChanged;

        Timer Timer { get; }

        TimeSpan Time { get; }

        Task<RecordedItem> CreateFileAsync();

        bool CanStartRecording();

        bool CanResumeRecording();

        void StartRecording(RecordedItem recordedItem, string subscriptionKey);

        void ResumeRecording();

        void StopRecording();
    }
}
