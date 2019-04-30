using System;
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
                    await _fileItemRepository.UpdateAllAsync(httpRequestResult.Payload).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.FileItemSynchronization, DateTime.UtcNow);
                }
            }
        }

        public async Task<IEnumerable<FileItem>> GetAllAsync()
        {
            return await _fileItemRepository.GetAllAsync().ConfigureAwait(false);
        }

        public async Task UploadAsync(MediaFile mediaFile)
        {
            var httpRequestResult = await _rewriteMeWebService.CreateFileItemAsync(mediaFile).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                await _fileItemRepository.InsertOrReplaceAsync(httpRequestResult.Payload).ConfigureAwait(false);
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

        public async Task TranscribeAsync(MediaFile mediaFile)
        {
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
