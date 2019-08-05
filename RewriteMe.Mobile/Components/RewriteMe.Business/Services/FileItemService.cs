using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Exceptions;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.Transcription;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class FileItemService : IFileItemService
    {
        private readonly IDeletedFileItemService _deletedFileItemService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly IInternalValueService _internalValueService;
        private readonly IFileItemRepository _fileItemRepository;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly ILogger _logger;

        public event EventHandler TranscriptionStarted;

        public FileItemService(
            IDeletedFileItemService deletedFileItemService,
            IUserSubscriptionService userSubscriptionService,
            IInternalValueService internalValueService,
            IFileItemRepository fileItemRepository,
            IRewriteMeWebService rewriteMeWebService,
            ILoggerFactory loggerFactory)
        {
            _deletedFileItemService = deletedFileItemService;
            _userSubscriptionService = userSubscriptionService;
            _internalValueService = internalValueService;
            _fileItemRepository = fileItemRepository;
            _rewriteMeWebService = rewriteMeWebService;
            _logger = loggerFactory.CreateLogger(typeof(FileItemService));
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate)
        {
            var lastFileItemSynchronizationTicks = await _internalValueService.GetValueAsync(InternalValues.FileItemSynchronizationTicks).ConfigureAwait(false);
            var lastFileItemSynchronization = new DateTime(lastFileItemSynchronizationTicks);
            _logger.Debug($"Update file items with timestamp '{lastFileItemSynchronization.ToString("d", CultureInfo.InvariantCulture)}'.");

            if (applicationUpdateDate >= lastFileItemSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetFileItemsAsync(lastFileItemSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    var fileItems = httpRequestResult.Payload.ToList();
                    _logger.Info($"Web server returned {fileItems.Count} items for synchronization.");

                    await _fileItemRepository.InsertOrReplaceAllAsync(fileItems).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.FileItemSynchronizationTicks, DateTime.UtcNow.Ticks).ConfigureAwait(false);
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

        public async Task DeleteAsync(FileItem fileItem)
        {
            var httpRequestResult = await _rewriteMeWebService.DeleteFileItemAsync(fileItem.Id).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                await _internalValueService.UpdateValueAsync(InternalValues.DeletedFileItemsTotalTime, httpRequestResult.Payload).ConfigureAwait(false);
            }
            else
            {
                var deletedFileItem = new DeletedFileItem
                {
                    Id = fileItem.Id,
                    DeletedDate = DateTime.UtcNow,
                    RecognitionState = fileItem.RecognitionState,
                    TotalTime = fileItem.TotalTime,
                    TranscribedTime = fileItem.TranscribedTime
                };

                await _deletedFileItemService.InsertAsync(deletedFileItem).ConfigureAwait(false);
            }

            await _fileItemRepository.DeleteAsync(fileItem.Id).ConfigureAwait(false);
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

        public async Task<bool> CanTranscribeAsync(TimeSpan fileTime)
        {
            var remainingSubscriptionTime = await _userSubscriptionService.GetRemainingTimeAsync().ConfigureAwait(false);
            var remainingTime = remainingSubscriptionTime.Subtract(fileTime);

            return remainingTime.Ticks >= 0;
        }

        public async Task TranscribeAsync(Guid fileItemId, string language)
        {
            var fileItem = await _fileItemRepository.GetAsync(fileItemId).ConfigureAwait(false);
            var canTranscribeFileItem = await CanTranscribeAsync(fileItem.TotalTime).ConfigureAwait(false);
            if (!canTranscribeFileItem)
                throw new NoSubscritionFreeTimeException();

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
