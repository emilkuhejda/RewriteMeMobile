using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRecorderService
    {
        Task<RecordedItem> CreateFileAsync();

        bool CanStartRecording();

        bool CanResumeRecording();

        void StartRecording(RecordedItem recordedItem, string subscriptionKey);

        void ResumeRecording();

        void StopRecording();
    }
}
