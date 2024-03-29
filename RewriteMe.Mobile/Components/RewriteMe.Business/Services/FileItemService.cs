﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RewriteMe.Business.Extensions;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Events;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class FileItemService : IFileItemService
    {
        private const int UploadedChunkSize = 1024 * 1024;

        private readonly IDeletedFileItemService _deletedFileItemService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IInternalValueService _internalValueService;
        private readonly IFileItemRepository _fileItemRepository;
        private readonly IUploadedSourceRepository _uploadedSourceRepository;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly ILogger _logger;

        private int _totalResourceInitializationTasks;
        private int _resourceInitializationTasksDone;

        public event EventHandler<UploadProgressEventArgs> UploadProgress;

        public FileItemService(
            IDeletedFileItemService deletedFileItemService,
            IUserSubscriptionService userSubscriptionService,
            IInternalValueService internalValueService,
            IFileItemRepository fileItemRepository,
            IUploadedSourceRepository uploadedSourceRepository,
            IRewriteMeWebService rewriteMeWebService,
            ILoggerFactory loggerFactory)
        {
            _deletedFileItemService = deletedFileItemService;
            _userSubscriptionService = userSubscriptionService;
            _internalValueService = internalValueService;
            _fileItemRepository = fileItemRepository;
            _uploadedSourceRepository = uploadedSourceRepository;
            _rewriteMeWebService = rewriteMeWebService;
            _logger = loggerFactory.CreateLogger(typeof(FileItemService));
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastFileItemSynchronizationTicks = await _internalValueService.GetValueAsync(InternalValues.FileItemSynchronizationTicks).ConfigureAwait(false);
            var lastFileItemSynchronization = new DateTime(lastFileItemSynchronizationTicks);
            _logger.Debug($"Update file items with timestamp '{lastFileItemSynchronization.ToString("d", CultureInfo.InvariantCulture)}'.");

            if (applicationUpdateDate > lastFileItemSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetFileItemsAsync(lastFileItemSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    var fileItems = httpRequestResult.Payload.ToList();
                    _logger.Info($"Web server returned {fileItems.Count} items for synchronization.");

                    await _fileItemRepository.InsertOrReplaceAllAsync(fileItems).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.FileItemSynchronizationTicks, applicationUpdateDate.Ticks).ConfigureAwait(false);
                }
            }
        }

        public async Task<FileItem> GetAsync(Guid fileItemId)
        {
            return await _fileItemRepository.GetAsync(fileItemId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FileItem>> GetAllAsync()
        {
            return await _fileItemRepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(FileItem fileItem)
        {
            var httpRequestResult = await _rewriteMeWebService.DeleteFileItemAsync(fileItem.Id).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Error && httpRequestResult.ErrorCode == ErrorCode.EC104)
                throw new FileNotUploadedException();

            if (httpRequestResult.State != HttpRequestState.Success)
            {
                var deletedFileItem = new DeletedFileItem
                {
                    Id = fileItem.Id,
                    DeletedDate = DateTime.UtcNow
                };

                await _deletedFileItemService.InsertAsync(deletedFileItem).ConfigureAwait(false);
            }

            await _fileItemRepository.DeleteAsync(fileItem.Id).ConfigureAwait(false);
        }

        public async Task<FileItem> CreateAsync(MediaFile mediaFile, CancellationToken cancellationToken)
        {
            var httpRequestResult = await _rewriteMeWebService.CreateFileItemAsync(mediaFile, cancellationToken).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                var fileItem = httpRequestResult.Payload;
                await _fileItemRepository.InsertOrReplaceAsync(fileItem).ConfigureAwait(false);
                return fileItem;
            }

            if (httpRequestResult.State == HttpRequestState.Error)
            {
                throw new ErrorRequestException(httpRequestResult.ErrorCode);
            }

            throw new OfflineRequestException();
        }

        public async Task<bool> UploadSourceFileAsync(Guid fileItemId, byte[] source, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var sourceParts = source.Split(UploadedChunkSize);
            var fileChunks = new List<FileChunk>();
            var order = 1;
            foreach (var sourcePart in sourceParts)
            {
                fileChunks.Add(new FileChunk(fileItemId, order++, sourcePart.ToArray()));
            }

            _totalResourceInitializationTasks = fileChunks.Count;
            _resourceInitializationTasksDone = 0;

            cancellationToken.ThrowIfCancellationRequested();

            var fileChunkRequests = fileChunks.ToArray().Split(10);
            foreach (var requests in fileChunkRequests)
            {
                var updateMethods = new List<Func<Task<bool>>>();
                foreach (var fileChunk in requests)
                {
                    updateMethods.Add(() => UploadChunkAsync(fileChunk, cancellationToken));
                }

                var tasks = updateMethods.WhenTaskDone(() => UpdateUploadProgress(fileItemId)).Select(x => x());
                var result = await Task.WhenAll(tasks).ConfigureAwait(false);

                var isSuccess = result.All(x => x);
                if (!isSuccess)
                {
                    throw new FileChunkNotUploadedUploadException();
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            var httpRequestResult = await _rewriteMeWebService.SubmitChunksAsync(fileItemId, fileChunks.Count, cancellationToken).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Error)
                throw new ErrorRequestException(httpRequestResult.ErrorCode);

            if (httpRequestResult.State != HttpRequestState.Success)
                throw new OfflineRequestException();

            cancellationToken.ThrowIfCancellationRequested();

            var fileItem = httpRequestResult.Payload;
            await _fileItemRepository.UpdateAsync(fileItem).ConfigureAwait(false);

            return true;
        }

        private void UpdateUploadProgress(Guid fileItemId)
        {
            var currentTask = Interlocked.Increment(ref _resourceInitializationTasksDone);
            OnUploadProgress(fileItemId, _totalResourceInitializationTasks, currentTask);
        }

        public async Task DeleteChunksAsync(Guid fileItemId)
        {
            var httpRequestResult = await _rewriteMeWebService.DeleteChunksAsync(fileItemId).ConfigureAwait(false);
            if (httpRequestResult.State != HttpRequestState.Success)
                throw new OfflineRequestException();
        }

        private async Task<bool> UploadChunkAsync(FileChunk fileChunk, CancellationToken cancellationToken)
        {
            var httpRequestResult = await _rewriteMeWebService.UploadChunkFileAsync(fileChunk.FileItemId, fileChunk.Order, fileChunk.Source, cancellationToken).ConfigureAwait(false);
            return httpRequestResult.State == HttpRequestState.Success;
        }

        public async Task<bool> CanTranscribeAsync()
        {
            var remainingSubscriptionTime = await _userSubscriptionService.GetRemainingTimeAsync().ConfigureAwait(false);
            return remainingSubscriptionTime.TotalSeconds >= 15;
        }

        public async Task TranscribeAsync(Guid fileItemId, string language, bool isPhoneCall, int transcriptionStartTimeSeconds, int transcriptionEndTimeSeconds)
        {
            var canTranscribeFileItem = await CanTranscribeAsync().ConfigureAwait(false);
            if (!canTranscribeFileItem)
                throw new NoSubscritionFreeTimeException();

            var httpRequestResult = await _rewriteMeWebService.TranscribeFileItemAsync(fileItemId, language, isPhoneCall, transcriptionStartTimeSeconds, transcriptionEndTimeSeconds).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                await _fileItemRepository.UpdateRecognitionStateAsync(fileItemId, RecognitionState.InProgress).ConfigureAwait(false);
            }
            else if (httpRequestResult.State == HttpRequestState.Error)
            {
                throw new ErrorRequestException(httpRequestResult.ErrorCode);
            }
            else
            {
                throw new OfflineRequestException();
            }
        }

        public async Task UpdateUploadStatusAsync(Guid fileItemId, UploadStatus uploadStatus)
        {
            await _fileItemRepository.UpdateUploadStatusAsync(fileItemId, uploadStatus).ConfigureAwait(false);
        }

        public async Task SetUploadErrorCodeAsync(Guid fileItemId, ErrorCode? errorCode)
        {
            await _fileItemRepository.SetUploadErrorCodeAsync(fileItemId, errorCode).ConfigureAwait(false);
        }

        public async Task SetTranscribeErrorCodeAsync(Guid fileItemId, ErrorCode? errorCode)
        {
            await _fileItemRepository.SetTranscribeErrorCodeAsync(fileItemId, errorCode).ConfigureAwait(false);
        }

        public async Task ResetUploadStatusesAsync()
        {
            await _fileItemRepository.ResetUploadStatusesAsync().ConfigureAwait(false);
            await _uploadedSourceRepository.ClearAsync().ConfigureAwait(false);
        }

        private void OnUploadProgress(Guid fileItemId, int totalSteps, int stepsDone)
        {
            UploadProgress?.Invoke(this, new UploadProgressEventArgs(fileItemId, totalSteps, stepsDone));
        }

        private class FileChunk
        {
            public FileChunk(Guid fileItemId, int order, byte[] source)
            {
                FileItemId = fileItemId;
                Order = order;
                Source = source;
            }

            public Guid FileItemId { get; }

            public int Order { get; }

            public byte[] Source { get; }
        }
    }
}
