﻿using System;
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
        private const string DirectoryName = "Records";

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

        public async Task<RecordedItem> CreateRecordedItemAsync(Guid fileId)
        {
            var path = GetAudioFilePath(fileId.ToString());
            Directory.CreateDirectory(path);

            var recordedItem = new RecordedItem
            {
                Id = fileId,
                FileName = fileId.ToString(),
                Path = path,
                DateCreated = DateTime.UtcNow
            };

            await _recordedItemRepository.InsertAsync(recordedItem).ConfigureAwait(false);
            return recordedItem;
        }

        public async Task DeleteRecordedItemAsync(Guid recordedItemId)
        {
            await _recordedItemRepository.DeleteAsync(recordedItemId).ConfigureAwait(false);

            var path = GetAudioFilePath(recordedItemId.ToString());
            Directory.Delete(path, true);
        }

        public async Task<IEnumerable<RecordedItem>> GetAllAsync()
        {
            return await _recordedItemRepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task InsertAudioFileAsync(RecordedAudioFile recordedAudioFile)
        {
            await _recordedAudioFileRepository.InsertAsync(recordedAudioFile).ConfigureAwait(false);
        }

        public async Task UpdateAudioFilesAsync(IEnumerable<RecordedAudioFile> recordedAudioFiles)
        {
            await _recordedAudioFileRepository.UpdateAllAsync(recordedAudioFiles).ConfigureAwait(false);
        }

        public void CreateDirectory()
        {
            var directoryPath = GetAudioDirectory();
            if (Directory.Exists(directoryPath))
                return;

            Directory.CreateDirectory(directoryPath);
        }

        public async Task ClearAsync()
        {
            await _recordedItemRepository.ClearAsync().ConfigureAwait(false);

            var directoryPath = GetAudioDirectory();
            if (!Directory.Exists(directoryPath))
                return;

            Directory.Delete(directoryPath, true);
        }

        public string GetAudioFilePath(string directoryName)
        {
            var directory = GetAudioDirectory();
            return Path.Combine(directory, directoryName);
        }

        public string GetAudioDirectory()
        {
            //var directory = _directoryProvider.GetPath();
            var directory = $"/storage/emulated/0/Download/";
            return Path.Combine(directory, DirectoryName);
        }
    }
}
