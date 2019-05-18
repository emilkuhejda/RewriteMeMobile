using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using RewriteMe.Domain.Configuration;
using RewriteMe.Domain.Http;
using RewriteMe.Domain.Interfaces.Repositories;
using RewriteMe.Domain.Interfaces.Services;
using RewriteMe.Domain.WebApi.Models;
using RewriteMe.Logging.Extensions;
using RewriteMe.Logging.Interfaces;

namespace RewriteMe.Business.Services
{
    public class DeletedFileItemService : IDeletedFileItemService
    {
        private readonly IInternalValueService _internalValueService;
        private readonly IDeletedFileItemRepository _deletedFileItemRepository;
        private readonly IFileItemRepository _fileItemRepository;
        private readonly IRewriteMeWebService _rewriteMeWebService;
        private readonly ILogger _logger;

        public DeletedFileItemService(
            IInternalValueService internalValueService,
            IDeletedFileItemRepository deletedFileItemRepository,
            IFileItemRepository fileItemRepository,
            IRewriteMeWebService rewriteMeWebService,
            ILoggerFactory loggerFactory)
        {
            _internalValueService = internalValueService;
            _deletedFileItemRepository = deletedFileItemRepository;
            _fileItemRepository = fileItemRepository;
            _rewriteMeWebService = rewriteMeWebService;
            _logger = loggerFactory.CreateLogger(typeof(DeletedFileItemService));
        }

        public async Task SynchronizationAsync(DateTime applicationUpdateDate, DateTime lastFileItemSynchronization)
        {
            if (lastFileItemSynchronization == default(DateTime))
            {
                await _internalValueService.UpdateValueAsync(InternalValues.DeletedFileItemSynchronization, DateTime.UtcNow).ConfigureAwait(false);
                return;
            }

            var lastDeletedFileItemSynchronization = await _internalValueService.GetValueAsync(InternalValues.DeletedFileItemSynchronization).ConfigureAwait(false);
            _logger.Debug($"Synchronization of deleted file items with timestamp '{lastFileItemSynchronization.ToString("d", CultureInfo.InvariantCulture)}'.");

            if (applicationUpdateDate >= lastDeletedFileItemSynchronization)
            {
                var httpRequestResult = await _rewriteMeWebService.GetDeletedFileItemIdsAsync(lastFileItemSynchronization).ConfigureAwait(false);
                if (httpRequestResult.State == HttpRequestState.Success)
                {
                    var fileItemIds = httpRequestResult.Payload.ToList();
                    _logger.Info($"Web server returned {fileItemIds.Count} items for synchronization.");

                    await _fileItemRepository.DeleteAsync(fileItemIds).ConfigureAwait(false);
                    await _internalValueService.UpdateValueAsync(InternalValues.DeletedFileItemSynchronization, DateTime.UtcNow).ConfigureAwait(false);
                }
            }
        }

        public async Task InsertAsync(DeletedFileItem deletedFileItem)
        {
            await _deletedFileItemRepository.InsertAsync(deletedFileItem).ConfigureAwait(false);
        }

        public async Task SendPendingAsync()
        {
            var pendingDeletedFileItems = await _deletedFileItemRepository.GetAllAsync().ConfigureAwait(false);
            var pendingFileItems = pendingDeletedFileItems.ToList();
            if (!pendingFileItems.Any())
                return;

            var httpRequestResult = await _rewriteMeWebService.DeleteAllFileItemAsync(pendingFileItems).ConfigureAwait(false);
            if (httpRequestResult.State == HttpRequestState.Success)
            {
                await _deletedFileItemRepository.ClearAsync().ConfigureAwait(false);
            }
        }
    }
}
