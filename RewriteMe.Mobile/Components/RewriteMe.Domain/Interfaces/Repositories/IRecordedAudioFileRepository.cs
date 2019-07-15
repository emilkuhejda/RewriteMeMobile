using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Repositories
{
    public interface IRecordedAudioFileRepository
    {
        Task InsertAsync(RecordedAudioFile recordedItem);

        Task UpdateAsync(RecordedAudioFile recordedItem);

        Task UpdateAllAsync(IEnumerable<RecordedAudioFile> recordedAudioFiles);
    }
}
