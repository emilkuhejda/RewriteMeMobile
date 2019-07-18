using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Required;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;

namespace RewriteMe.Business.Services
{
    public class RecordedItemService : IRecordedItemService
    {
        private const string DirectoryName = "Recordings";

        private readonly IDirectoryProvider _directoryProvider;
        private readonly IRecordedItemRepository _recordedItemRepository;
        private readonly IRecordedAudioFileRepository _recordedAudioFileRepository;

        public RecordedItemService(
            IDirectoryProvider directoryProvider,
            IRecordedItemRepository recordedItemRepository,
            IRecordedAudioFileRepository recordedAudioFileRepository)
        {
            _directoryProvider = directoryProvider;
            _recordedItemRepository = recordedItemRepository;
            _recordedAudioFileRepository = recordedAudioFileRepository;
        }

        public async Task<RecordedItem> CreateRecordedItemAsync(bool isRecordingOnly)
        {
            var recordedItem = new RecordedItem
            {
                Id = Guid.NewGuid(),
                FileName = DateTime.UtcNow.ToString("dd-MM-yyyy_HH_mm_ss"),
                IsRecordingOnly = isRecordingOnly,
                DateCreated = DateTime.UtcNow
            };

            await _recordedItemRepository.InsertAsync(recordedItem).ConfigureAwait(false);
            return recordedItem;
        }

        public async Task DeleteRecordedItemAsync(RecordedItem recordedItem)
        {
            await _recordedItemRepository.DeleteAsync(recordedItem.Id).ConfigureAwait(false);

            var filePath = GetAudioPath(recordedItem);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<RecordedItem> GetAsync(Guid recordedItemId)
        {
            return await _recordedItemRepository.GetAsync(recordedItemId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<RecordedItem>> GetAllAsync()
        {
            return await _recordedItemRepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task InsertAudioFileAsync(RecordedAudioFile recordedAudioFile)
        {
            await _recordedAudioFileRepository.InsertAsync(recordedAudioFile).ConfigureAwait(false);
        }

        public async Task UpdateAudioFileAsync(RecordedAudioFile recordedAudioFile)
        {
            await _recordedAudioFileRepository.UpdateAsync(recordedAudioFile).ConfigureAwait(false);
        }

        public async Task UpdateAudioFilesAsync(IEnumerable<RecordedAudioFile> recordedAudioFiles)
        {
            await _recordedAudioFileRepository.UpdateAllAsync(recordedAudioFiles).ConfigureAwait(false);
        }

        public void CreateDirectory()
        {
            var directoryPath = GetDirectoryPath();
            if (Directory.Exists(directoryPath))
                return;

            Directory.CreateDirectory(directoryPath);
        }

        public string GetDirectoryPath()
        {
            var directory = _directoryProvider.GetPath();
            return Path.Combine(directory, DirectoryName);
        }

        public string GetAudioPath(RecordedItem recordedItem)
        {
            if (!recordedItem.IsRecordingOnly)
                return null;

            return Path.Combine(GetDirectoryPath(), recordedItem.AudioFileName);
        }
    }
}
