using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.DataAccess.DataAdapters;
using RewriteMe.DataAccess.Entities;
using RewriteMe.DataAccess.Providers;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi;

namespace RewriteMe.DataAccess.Repositories
{
    public class FileItemRepository : IFileItemRepository
    {
        private readonly IAppDbContextProvider _contextProvider;

        public FileItemRepository(IAppDbContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }

        public async Task<IEnumerable<FileItem>> GetAllAsync()
        {
            var entities = await _contextProvider.Context.FileItems.ToListAsync().ConfigureAwait(false);
            return entities.Select(x => x.ToFileItem());
        }

        public async Task<FileItem> GetAsync(Guid fileItemId)
        {
            var entity = await _contextProvider.Context.GetAsync<FileItemEntity>(x => x.Id == fileItemId).ConfigureAwait(false);

            return entity?.ToFileItem();
        }

        public async Task<IEnumerable<FileItem>> GetUploadingFilesAsync()
        {
            var entities = await _contextProvider.Context.FileItems
                .Where(x => x.UploadStatus == UploadStatus.InProgress)
                .ToListAsync()
                .ConfigureAwait(false);

            return entities.Select(x => x.ToFileItem());
        }

        public async Task<bool> AnyWaitingForSynchronizationAsync()
        {
            var recognitionState = RecognitionState.InProgress;
            var count = await _contextProvider.Context.FileItems
                .Where(x => x.RecognitionState == recognitionState)
                .CountAsync()
                .ConfigureAwait(false);

            return count > 0;
        }

        public async Task InsertOrReplaceAsync(FileItem fileItem)
        {
            await _contextProvider.Context.InsertOrReplaceAsync(fileItem.ToFileItemEntity()).ConfigureAwait(false);
        }

        public async Task InsertOrReplaceAllAsync(IEnumerable<FileItem> fileItems)
        {
            var fileItemEntities = fileItems.Select(x => x.ToFileItemEntity()).ToList();
            if (!fileItemEntities.Any())
                return;

            var existingEntities = await _contextProvider.Context.FileItems.ToListAsync().ConfigureAwait(false);
            var mergedFileItems = existingEntities.Where(x => fileItemEntities.All(e => e.Id != x.Id)).ToList();
            mergedFileItems.AddRange(fileItemEntities);

            await _contextProvider.Context.RunInTransactionAsync(database =>
            {
                database.DeleteAll<FileItemEntity>();
                database.InsertAll(mergedFileItems);
            }).ConfigureAwait(false);
        }

        public async Task DeleteAsync(IEnumerable<Guid> fileItemIds)
        {
            foreach (var fileItemId in fileItemIds)
            {
                await _contextProvider.Context.DeleteAsync<FileItemEntity>(fileItemId).ConfigureAwait(false);
            }
        }

        public async Task DeleteAsync(Guid fileItemId)
        {
            await _contextProvider.Context.DeleteAsync<FileItemEntity>(fileItemId).ConfigureAwait(false);
        }

        public async Task UpdateAsync(FileItem fileItem)
        {
            var entity = fileItem.ToFileItemEntity();
            await _contextProvider.Context.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task UpdateRecognitionStateAsync(Guid fileItemId, RecognitionState recognitionState)
        {
            var entity = await _contextProvider.Context.GetAsync<FileItemEntity>(x => x.Id == fileItemId).ConfigureAwait(false);
            if (entity == null)
                return;

            entity.RecognitionState = recognitionState;
            await _contextProvider.Context.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task UpdateUploadStatusAsync(Guid fileItemId, UploadStatus uploadStatus)
        {
            var entity = await _contextProvider.Context.GetAsync<FileItemEntity>(x => x.Id == fileItemId).ConfigureAwait(false);
            if (entity == null)
                return;

            entity.UploadStatus = uploadStatus;
            await _contextProvider.Context.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task SetUploadErrorCodeAsync(Guid fileItemId, ErrorCode errorCode)
        {
            var entity = await _contextProvider.Context.GetAsync<FileItemEntity>(x => x.Id == fileItemId).ConfigureAwait(false);
            if (entity == null)
                return;

            entity.UploadErrorCode = errorCode;
            await _contextProvider.Context.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task SetTranscribeErrorCodeAsync(Guid fileItemId, ErrorCode errorCode)
        {
            var entity = await _contextProvider.Context.GetAsync<FileItemEntity>(x => x.Id == fileItemId).ConfigureAwait(false);
            if (entity == null)
                return;

            entity.TranscribeErrorCode = errorCode;
            await _contextProvider.Context.UpdateAsync(entity).ConfigureAwait(false);
        }

        public async Task ClearAsync()
        {
            await _contextProvider.Context.DeleteAllAsync<FileItemEntity>().ConfigureAwait(false);
        }
    }
}
