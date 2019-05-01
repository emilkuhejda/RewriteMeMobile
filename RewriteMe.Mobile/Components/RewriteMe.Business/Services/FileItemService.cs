﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;

namespace RewriteMe.Business.Services
{
    public class FileItemService : IFileItemService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IFileItemRepository _fileItemRepository;
        private readonly IRewriteMeWebService _rewriteMeWebService;

        public event EventHandler TranscriptionStarted;

        public FileItemService(
            IInternalValueService internalValueService,
            IFileItemRepository fileItemRepository,
            IRewriteMeWebService rewriteMeWebService)
        {
            _internalValueService = internalValueService;
            _fileItemRepository = fileItemRepository;
            _rewriteMeWebService = rewriteMeWebService;
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastFileItemSynchronization = await _internalValueService.GetValueAsync(InternalValues.FileItemSynchronization).ConfigureAwait(false);
            if (applicationUpdateDate >= lastFileItemSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetFileItemsAsync(lastFileItemSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    await _fileItemRepository.InsertOrReplaceAllAsync(httpRequestResult.Payload).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.FileItemSynchronization, DateTime.UtcNow);
                }
            }
        }

        public async Task<bool> AnyWaitingForSynchronizationAsync()
        {
            return await _fileItemRepository.AnyWaitingForSynchronizationAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<FileItem>> GetAllAsync()
        {
            return await _fileItemRepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task<FileItem> UploadAsync(MediaFile mediaFile)
        {
            var httpRequestResult = await _rewriteMeWebService.UploadFileItemAsync(mediaFile).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                var fileItem = httpRequestResult.Payload;
                await _fileItemRepository.InsertOrReplaceAsync(fileItem).ConfigureAwait(false);
                return fileItem;
            }

            if (httpRequestResult.State == HttpRequestState.Error)
            {
                throw new ErrorRequestException(httpRequestResult.StatusCode);
            }

            throw new OfflineRequestException();
        }

        public async Task TranscribeAsync(Guid fileItemId, string language)
        {
            var httpRequestResult = await _rewriteMeWebService.TranscribeFileItemAsync(fileItemId, language).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                await _fileItemRepository.UpdateRecognitionStateAsync(fileItemId, RecognitionState.InProgress).ConfigureAwait(false);

                OnTranscriptionStarted();
            }
            else if (httpRequestResult.State == HttpRequestState.Error)
            {
                throw new ErrorRequestException(httpRequestResult.StatusCode);
            }
            else
            {
                throw new OfflineRequestException();
            }
        }

        private void OnTranscriptionStarted()
        {
            TranscriptionStarted?.Invoke(this, EventArgs.Empty);
        }
    }
}
