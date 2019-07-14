using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Domain.Interfaces.Services
{
    public interface IRecordedItemService
    {
        Task<RecordedItem> CreateRecordedItemAsync(Guid fileId);

        Task DeleteRecordedItemAsync(Guid recordedItemId);

        Task<IEnumerable<RecordedItem>> GetAllAsync();

        Task InsertAudioFileAsync(RecordedAudioFile recordedAudioFile);

        Task UpdateAudioFilesAsync(IEnumerable<RecordedAudioFile> recordedAudioFiles);

        void CreateDirectory();

        Task ClearAsync();

        string GetAudioFilePath(string directoryName);

        string GetAudioDirectory();
    }
}
