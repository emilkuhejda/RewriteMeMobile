using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRecordedItemService
    {
        Task<RecordedItem> CreateRecordedItemAsync(bool isRecordingOnly);

        Task DeleteRecordedItemAsync(RecordedItem recordedItem);

        Task<RecordedItem> GetAsync(Guid recordedItemId);

        Task<IEnumerable<RecordedItem>> GetAllAsync();

        Task InsertAudioFileAsync(RecordedAudioFile recordedAudioFile);

        Task UpdateAudioFileAsync(RecordedAudioFile recordedAudioFile);

        Task UpdateAudioFilesAsync(IEnumerable<RecordedAudioFile> recordedAudioFiles);

        void CreateDirectory();

        string GetDirectoryPath();

        string GetAudioPath(RecordedItem recordedItem);
    }
}
